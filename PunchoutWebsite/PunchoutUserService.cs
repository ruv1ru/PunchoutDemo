using System;
namespace PunchoutWebsite
{
    public static class PunchoutUserService
    {
        // PunchoutUserTokenService will create new user token in the repository upon successful setup
        // and return it when validating the request made by procurement system user to login to this system


        /// <summary>
        /// Gets the unique customer token of specific user who is punching in to the system.
        /// This is a unique value that is generated on each succesfull setup request
        /// </summary>
        /// <returns>The customer token.</returns>
        public static string GetUserToken()
        {
            //return Guid.NewGuid();
            return "7d92c2ed-32ae-4bd4-9453-1a337bd7cb33";
        }
        /// <summary>
        /// Gets the user procurement system submit URL (sent by user as a part of the setup request) saved in repository.
        /// </summary>
        /// <returns>The user procurement system submit URL.</returns>
        public static string GetProcurementSystemPostUrl()
        {
            return "https://bigbuyer.com:2600/punchout?client=NAwl4Jo";
        }





        /// <summary>
        /// On successful setup request, gets the URL sent by procurement system and save in repostory
        /// </summary>
        /// <param name="url">URL</param>
        public static void SaveProcurementSystemPostUrl(string url)
        {
            // save url in repository
        }


        /// <summary>
        /// Gets the user shared secret that is securely stored in repository
        /// </summary>
        /// <returns>The user shared secret.</returns>
        public static string GetUserSharedSecret()
        {
            
            return "abracadabra";
        }
    }


}


