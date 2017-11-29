using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwensCorning.Utility.Logging;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Net;
using System.Data.Linq;
using System.Diagnostics;

namespace OwensCorning.NotificationService
{
    public class DocumentService
    {
        private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int DaysToCheckIfNoPreviousBatch
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["DaysToCheckIfNoPreviousBatchRun"]); }
        }

        private DateTime lastBatchRunDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        #region New Stuff
        /// <summary>
        /// Processes the ektron documents 
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        public List<UpdatedDoc> GetUpdatedDocumentList(DocumentProcessOptions options)
        {
            return GetDocumentsFromEktron(options);
        }

        private List<UpdatedDoc> GetDocumentsFromEktron(DocumentProcessOptions processingOptions)
        {
            if (null == processingOptions)
                throw new ArgumentNullException("processingOptions");
            if (String.IsNullOrEmpty(processingOptions.Site))
                throw new ArgumentOutOfRangeException("processingOptions.Site");

            if (String.IsNullOrEmpty(processingOptions.TaxonomyName) && !String.IsNullOrEmpty(processingOptions.Site))
            {
                processingOptions.TaxonomyName = GetTaxonomyName(processingOptions.Site);
            }


            DateTime startDate = (DateTime.MinValue.Equals(processingOptions.StartDate)) ? System.Data.SqlTypes.SqlDateTime.MinValue.Value : processingOptions.StartDate;
            DateTime endDate = (DateTime.MinValue.Equals(processingOptions.EndDate) || processingOptions.EndDate > System.Data.SqlTypes.SqlDateTime.MaxValue.Value) ? System.Data.SqlTypes.SqlDateTime.MaxValue.Value : processingOptions.EndDate;

            try
            {
                using (EktronDataContext ektronDataContext = new EktronDataContext())
                {

                    Taxonomy parentTaxonomy = ektronDataContext.Taxonomies.FirstOrDefault(tax => tax.taxonomy_name == processingOptions.TaxonomyName);
                    long metaTypeId = (from metatype in ektronDataContext.metadata_types where metatype.meta_name == "DocumentFileSize" select metatype.meta_type_id).FirstOrDefault();

                    var taxonomyItems = (from childTaxonomy in ektronDataContext.Taxonomies
                                         join taxonomyItem in ektronDataContext.TaxonomyItems on childTaxonomy.taxonomy_id equals taxonomyItem.taxonomy_id
                                         join sizeInfo in ektronDataContext.content_meta_tbls on taxonomyItem.EKContents.content_id equals sizeInfo.content_id
                                         where
                                         childTaxonomy.taxonomy_parent_id == parentTaxonomy.taxonomy_id
                                         && ((taxonomyItem.taxonomy_item_date_modified.CompareTo(startDate) >= 0
                                         && taxonomyItem.taxonomy_item_date_modified.CompareTo(endDate) <= 0)
                                         || (taxonomyItem.EKContents.last_edit_date.CompareTo(startDate) >= 0
                                         && taxonomyItem.EKContents.last_edit_date.CompareTo(endDate) <= 0))
                                         && sizeInfo.meta_type_id == metaTypeId
                                         && (null != taxonomyItem.EKContents.Document)
                                         && (null == processingOptions.FileTypes ||
                                            processingOptions.FileTypes.Contains(taxonomyItem.EKContents.Document.handle.Substring(taxonomyItem.EKContents.Document.handle.LastIndexOf(".") + 1).Trim().ToLower()))
                                         select new UpdatedDoc
                                         {
                                             url = "http://" + processingOptions.Site + "/assets/" + taxonomyItem.EKContents.Document.pubFolderPath + taxonomyItem.EKContents.Document.storage +
                                                 taxonomyItem.EKContents.Document.handle.Substring(taxonomyItem.EKContents.Document.handle.LastIndexOf(".")),
                                             dateUpdated = (DateTime.Compare(taxonomyItem.taxonomy_item_date_modified,taxonomyItem.EKContents.last_edit_date) > 0 ? taxonomyItem.taxonomy_item_date_modified : taxonomyItem.EKContents.last_edit_date),
                                             documentName = taxonomyItem.EKContents.content_title.Replace("’", "'").Replace("&amp;", "&").Replace("“", "\"").Replace("”","\""),// taxonomyItem.EKContents.Document.handle,
                                             documentType = childTaxonomy.taxonomy_name,
                                             fileSize = sizeInfo.meta_value
                                         }).Distinct();


                    System.Diagnostics.Debug.WriteLine("Documents Found = " + taxonomyItems.Count());
                    return taxonomyItems.ToList();

                }
            }
            catch (Exception e)
            {
                log.Error("Unable to retrieve Ektron Documents or create Updated Documents", e);
                throw e;
            }

        }

        public Batch CreateBatch(string site, DocumentProcessOptions options)
        {

            List<UpdatedDoc> itemsForBatch = GetDocumentsFromEktron(options);
            Batch batch = CreateBatchForItems(itemsForBatch, options);
            return batch;
        }

        private Batch CreateBatchForItems(List<UpdatedDoc> itemsForBatch, DocumentProcessOptions options)
        {
            //TODO: Transaction!
            using (NotificationTablesDataContext notificationDataContext = new NotificationTablesDataContext())
            {

                Batch batch = new Batch();
                batch.startDate = options.StartDate;
                batch.endDate = options.EndDate;
                batch.site = options.Site;

                notificationDataContext.Batches.InsertOnSubmit(batch);
                notificationDataContext.SubmitChanges();

                // Add the updated docs for this batch
                foreach (UpdatedDoc docToAddToBatch in itemsForBatch)
                {
                    //TODO: why not use UpdatedDocument here instead of Updated Doc
                    notificationDataContext.UpdatedDocuments.InsertOnSubmit
                        (
                        new UpdatedDocument
                        {
                            batch = batch.batchId,
                            dateUpdated = docToAddToBatch.dateUpdated,
                            documentType = docToAddToBatch.documentType,
                            documentName = docToAddToBatch.documentName,
                            fileSize = docToAddToBatch.fileSize,
                            url = docToAddToBatch.url
                        }
                        );
                };
                batch.finishDate = DateTime.Now;
                notificationDataContext.SubmitChanges();
                return batch;
            }

        }

        private string GetTaxonomyName(string site)
        {
            if (String.IsNullOrEmpty(site))
                throw new ArgumentNullException("site");
            using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
            {
                return dataContext.SiteConfigurations.FirstOrDefault(config => config.site == site).taxonomyName;
            }
        }

        #endregion

        #region Updated Items
        
        /// <summary>
        /// Processes the ektron documents for anything updated since last batch run.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        public DocumentProcessResults ProcessEktronDocuments(DocumentProcessOptions processingOptions, List<int> listExcludeFromProcessing)
        {
            if (null == processingOptions)
                throw new ArgumentNullException("processingOptions");
            if (String.IsNullOrEmpty(processingOptions.Site))
                throw new ArgumentOutOfRangeException("processingOptions.Site");

            try
            {
                List<UpdatedDoc> updateDocuments = GetDocumentsFromEktron(processingOptions);

                if (listExcludeFromProcessing.Count > 0)
                {
                    foreach (int item in listExcludeFromProcessing)
                    {
                        updateDocuments.RemoveAt(Convert.ToInt16(item));
                    }
                }

                if (updateDocuments.Count == 0)
                {
                    return new DocumentProcessResults
                    {
                        Status = "Success - No Documents to Proccess Batch was not created",
                        DocumentsInBatch = 0,
                        BatchId = 0,
                    };
                }

                Batch currentBatch = CreateBatchForItems(updateDocuments, processingOptions);
                return new DocumentProcessResults
                {
                    Status = "Success",
                    StartDate = currentBatch.startDate,
                    EndDate = currentBatch.endDate,
                    DocumentsInBatch = (null == updateDocuments) ? 0 : updateDocuments.Count,
                    BatchId = currentBatch.batchId,
                };
                
            }
            catch (Exception e)
            {
                String errorMessage = "Error: Something happened in the processing of documents, please see the errors above for clarification.";
                log.Error(errorMessage, e);

                return new DocumentProcessResults
                {
                    Status = String.Format("{0}: {1}", errorMessage, e),
                };
            }
        }

        public DocumentProcessResults ProcessEktronDocuments(DocumentProcessOptions processingOptions)
        {
            return ProcessEktronDocuments(processingOptions, new List<int>());
        }

        #endregion


        /// <summary>
        /// Gets the previous documents list by site and batch id.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="batch">The batch id.</param>
        /// <returns>A list of documents that were in a specific batch</returns>
        public BatchedDocumentResults GetDocumentsList(string site, int batchId)
        {
            try
            {
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {


                    Batch batch = null;
                    if (batchId <= 0)
                    {
                        batch = GetLastBatchRun(site);
                    }
                    else
                    {
                        batch = dataContext.Batches.FirstOrDefault(b => b.batchId == batchId && b.site == site);
                    }

                    List<UpdatedDoc> documentList =  (from docs in dataContext.UpdatedDocuments
                                       where docs.Batch1.site == site && docs.batch == batchId
                                       select new UpdatedDoc
                                       {
                                           batch = (int)docs.batch,
                                           dateUpdated = (DateTime)docs.dateUpdated,
                                           documentName = docs.documentName,
                                           documentType = docs.documentType,
                                           fileSize = docs.fileSize,
                                           url = docs.url
                                       }).ToList();

                    return new BatchedDocumentResults
                    {
                        Status = "Success",
                        BatchId = batchId,
                        StartDate = (null == batch)? DateTime.MinValue : batch.startDate,
                        EndDate = (null == batch) ? DateTime.MinValue : batch.endDate,
                        DocumentList = documentList
                    };
                }
            }
            catch (Exception e)
            {
                log.Error("Not able to get documents list", e);
                return new BatchedDocumentResults
                {
                    Status = "Error " + e.Message,
                    BatchId = batchId,
                };
            }
        }

        ///// <summary>
        ///// Gets the previous documents list by site.
        ///// </summary>
        ///// <param name="site">The site.</param>
        ///// <returns>A list of documents that were in the last run batch</returns>
        //public List<UpdatedDoc> GetDocumentsList(string site)
        //{
        //    try
        //    {
        //        int latestBatch = GetLastBatchRun(site).batchId;
        //        return GetDocumentsList(site, latestBatch);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        /// <summary>
        /// Creates the new batch.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>The new batch.</returns>
        private Batch CreateNewBatch(string site)
        {
            try
            {
                using (NotificationTablesDataContext notificationDataContext = new NotificationTablesDataContext())
                {

                    Batch batch = new Batch();
                    batch.startDate = DateTime.Now;
                    batch.site = site;

                    notificationDataContext.Batches.InsertOnSubmit(batch);
                    notificationDataContext.SubmitChanges();

                    return batch;
                }
            }
            catch (Exception e)
            {
                log.Error("Unable to create a new batch", e);
                throw e;
            }

        }

        /// <summary>
        /// Gets the last batch run by site.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>The latest batch</returns>
        public Batch GetLastBatchRun(string site)
        {
            try
            {
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    return dataContext.Batches.Where(batch => batch.site == site).OrderByDescending(batch => batch.batchId).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                log.Error("Unable to find the last batch run for site: " + site, e);
                throw e;
            }
        }


        /*
        /// <summary>
        /// Gets the updated ektron documents.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>A list of Ektron Documents</returns>
        private void LoadUpdateDocumentsFromEktron(string site, Batch batch, string taxonomyName)
        {
            List<Document> ektronDocuments = new List<Document>();

            try
            {
                    using (EktronDataContext ektronDataContext = new EktronDataContext())
                    {

                        Taxonomy parentTaxonomy = ektronDataContext.Taxonomies.FirstOrDefault(tax => tax.taxonomy_name == taxonomyName);
                        //List<Taxonomy> childTaxonomy = ektronDataContext.Taxonomies.Where(tax => tax.taxonomy_parent_id == parentTaxonomy.taxonomy_id ).ToList();

                        int metaTypeId = (from metatype in ektronDataContext.metadata_types where metatype.meta_name == "DocumentFileSize" select metatype.meta_type_id).FirstOrDefault();
                        var taxonomyItems = (from childTaxonomy in ektronDataContext.Taxonomies
                                             join taxonomyItem in ektronDataContext.TaxonomyItems on childTaxonomy.taxonomy_id equals taxonomyItem.taxonomy_id
                                             join sizeInfo in ektronDataContext.content_meta_tbls on taxonomyItem.EKContents.content_id equals sizeInfo.content_id 
                                             where 
                                             childTaxonomy.taxonomy_parent_id == parentTaxonomy.taxonomy_id
                                             && taxonomyItem.taxonomy_item_date_modified.CompareTo(lastBatchRunDate) >= 0
                                             && sizeInfo.meta_type_id == metaTypeId
                                             select new
                                             {
                                                 TaxonomyItem = taxonomyItem,
                                                 DocumentType = childTaxonomy.taxonomy_name,
                                                 Size = sizeInfo.meta_value
                                             }).Distinct();
                        //foreach (Taxonomy child in childTaxonomy)
                        //{
                            //List<TaxonomyItem> taxonomyItems = child.TaxonomyItems.Where(taxItem => taxItem.taxonomy_item_date_modified.CompareTo(lastBatchRunDate) >= 0).ToList();

                            using (NotificationTablesDataContext notificationDataContext = new NotificationTablesDataContext())
                            {

                                foreach (var taxItemThing in taxonomyItems)
                                {
                                    TaxonomyItem taxItem = taxItemThing.TaxonomyItem;
                                    string documentType = taxItemThing.DocumentType;

                                    DumpDocument(taxItem.EKContents);

                                    if (null == taxItem.EKContents.Document)
                                        continue;

                                    string documentUrl = "http://" + site + "/assets/" + taxItem.EKContents.Document.pubFolderPath + taxItem.EKContents.Document.storage + ".pdf";
                                    UpdatedDocument updatedDoc = new UpdatedDocument
                                    {
                                        url = documentUrl,
                                        batch = batch.batchId,
                                        dateUpdated = taxItem.taxonomy_item_date_modified,
                                        documentName = taxItem.EKContents.Document.handle,
                                        documentType = documentType,
                                        fileSize = taxItemThing.Size
                                    };
                                 
                                    notificationDataContext.UpdatedDocuments.InsertOnSubmit(updatedDoc);

                                }
                                notificationDataContext.SubmitChanges();
                                //}
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Unable to retrieve Ektron Documents or create Updated Documents", e);
                throw e;
            }
        }*/

        [Conditional("DEBUG")]
        private void DumpDocument(EKContent eKContent)
        {
            log.Info("Ektron Content Document ");
            if (null == eKContent.Document)
            {
                Debug.WriteLine("Document null " + eKContent.content_title);
                log.Debug("Unable to find the document for the content item: " + eKContent.content_title);
                return;
            }
        }

        /// <summary>
        /// Gets the last batch run date.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>DateTime the last batch was run or default date if no batch was run previously</returns>
        private DateTime GetLastBatchRunDate(string site)
        {
            try
            {
                Batch lastBatchRun = GetLastBatchRun(site);
                if (lastBatchRun == null)
                    return DateTime.Now.AddDays(DaysToCheckIfNoPreviousBatch * -1);
                else
                    return lastBatchRun.startDate;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
