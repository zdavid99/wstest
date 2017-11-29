using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwensCorning.Utility.Logging;
using System.Diagnostics;

namespace OwensCorning.NotificationService
{
    public class SubscriptionService
    {
        private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Subscribes the specified email from the specified site's notifications.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="site">The site.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <returns>
        /// true if subscribed / false if errors occured
        /// </returns>
        public static Boolean Subscribe(string email, string site, string firstName, string lastName)
        {
            try
            {
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    Subscription newSubscription = dataContext.Subscriptions.FirstOrDefault(sub => sub.email == email && sub.site == site);
                    //Has opted in before, may be opted out or not.
                    if (newSubscription != null)
                    {
                        if (firstName != null)
                            newSubscription.firstName = firstName;
                        if (lastName != null)
                            newSubscription.lastName = lastName;

                        newSubscription.optedIn = true;
                    }
                    //Is a new Subscription
                    else
                    {
                        newSubscription = new Subscription();
                        newSubscription.email = email;
                        newSubscription.site = site;
                        newSubscription.firstName = firstName;
                        newSubscription.lastName = lastName;
                        newSubscription.optedIn = true;

                        dataContext.Subscriptions.InsertOnSubmit(newSubscription);
                    }
                    dataContext.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                log.Error("Not able to create subscription for email: " + email, e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Subscribes the specified email from the specified site's notifications.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="site">The site.</param>
        /// <returns>
        /// true if subscribed / false if errors occured
        /// </returns>
        public static Boolean Subscribe(string email, string site)
        {
            return Subscribe(email, site, null, null);
        }

        /// <summary>
        /// Unsubscribes the specified email from the specified site's notifications.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        public static Boolean Unsubscribe(string email, string site)
        {
            try
            {
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    Subscription unSubscription = dataContext.Subscriptions.FirstOrDefault(sub => sub.email == email && sub.site == site);
                    if (unSubscription != null)
                    {
                        unSubscription.optedIn = false;
                        dataContext.SubmitChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Not able to unsubscribe " + email, e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets all subscribers.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>List of Subscription Objects</returns>
        public static List<Subscription> GetCurrentSubscribers(string site)
        {
            try
            {
                using (NotificationTablesDataContext dataContext = new NotificationTablesDataContext())
                {
                    List<Subscription> subscriptions = dataContext.Subscriptions.Where(subscription => subscription.site == site && subscription.optedIn).ToList();
                    DumpSubscriptions(subscriptions);
                    return subscriptions;
                }
            }
            catch (Exception e)
            {
                log.Error("Not able to get a list of current subscribers for site: " + site, e);
                return null;
            }
        }

        [Conditional("DEBUG")]
        private static void DumpSubscriptions(List<Subscription> subscriptions)
        {
            Debug.WriteLine("Subscriptions");
            foreach (Subscription subscription in subscriptions)
                System.Diagnostics.Debug.WriteLine(subscription.email);
            
        }
    }
}
