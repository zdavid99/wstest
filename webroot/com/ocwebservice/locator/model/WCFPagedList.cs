using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using OwensCorning.Paging;

namespace com.ocwebservice.locator.model
{
    /// <summary>
    /// IPaginatedEnumerable to publish over web service boundary
    /// </summary>
    [DataContract(Name="PaginatedList", Namespace="com.owenscorning.locator")]
    public class WCFPagedList<T> : OwensCorning.Paging.IPaginatedEnumerable<T>
    {
        [DataMember]
        public List<T> PageData { get; set; }

        public WCFPagedList()
        {
        }

        public WCFPagedList(IEnumerable<T> source, PageInfo pagingInfo)
        {
            var paginatedEnumerable = source.GetPage(pagingInfo);
            Pagination = paginatedEnumerable.Pagination;
            TotalItemCount = paginatedEnumerable.TotalItemCount;
            PageData = paginatedEnumerable.ToList();
        }

        #region IPaginatedEnumerable<T> Members
        [DataMember]
        public OwensCorning.Paging.PageInfo Pagination { get; set; }
        [DataMember]
        public int TotalItemCount { get; set; }
        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return PageData.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return PageData.GetEnumerator();
        }

        #endregion
    }
}
