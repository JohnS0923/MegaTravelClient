using Microsoft.AspNetCore.Mvc;
using MegaTravelClient.Models;
using Microsoft.AspNetCore.Authorization;
using MegaTravelClient.Utility;
using Newtonsoft.Json;

namespace MegaTravelClient.Controllers
{

    public class UserController : Controller
    {
        public IActionResult Index(LoginResponseModel userData)
        {

            return View();
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                //Use Api endpoint for getallusers that will return a list of users from the db
                User currentUser = null;

                GetUsersResponseModel resModel = null;

                var serializedData = string.Empty;
                ServiceHelper service = new ServiceHelper();
                string response = await service.GetRequest(serializedData, ConstantValues.GetAllUsersAPI, false,string.Empty).ConfigureAwait(true) ;

                resModel = JsonConvert.DeserializeObject<GetUsersResponseModel>(response);


                //search through list for the one user who has this userid
                foreach (User user in resModel.userList)
                {
                    if (user.UserId == id)
                    {
                        currentUser = user;
                        break;
                    }
                }
                //can also
                //currentUser = resModel.userList.FirstOrDefault(u => u.UserId == id);


                //send that user to the view so the fields can be populated
                return View(currentUser);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Get User Data"+ex.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Edit(User updateUser)
        {
            try
            {
                LoginResponseModel ResponseModel = null;
                //update data in the database
                //call the updateuser endpoint
                var serializedData = JsonConvert.SerializeObject(updateUser);
                ServiceHelper service = new ServiceHelper();
                //pass over the updated user object so it can be saved
                string res = await service.PostRequest(serializedData, ConstantValues.UpdateUser,false,string.Empty).ConfigureAwait(true);
                ResponseModel = JsonConvert.DeserializeObject<LoginResponseModel>(res);


                //evaluate a response from the api to see if it is successful
                if (ResponseModel == null)
                {
                    //error with an api
                }
                else if (ResponseModel.Status == false)
                {
                    //an error occured
                }
                else
                {
                    //if successful take user back to user dashboard
                    return View("Views/User/Index.cshtml",ResponseModel);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Edit API Call "+ ex.Message);
            }
            return View();
        }

        public async Task<ActionResult> Trips([FromQuery(Name = "userID")] int userID)
        {

            List<GetTripsResponseModel> userList = new List<GetTripsResponseModel>();

            GetTripsResponseModel ResponseModel = null;
            try
            {
                var strSerializedData = string.Empty;
                ServiceHelper objService = new ServiceHelper();
                string response = await objService.GetRequest(strSerializedData, ConstantValues.GetAllTripsByUser + "?userID=" + userID, false, string.Empty).ConfigureAwait(true);
                ResponseModel = JsonConvert.DeserializeObject<GetTripsResponseModel>(response);
                ResponseModel.userId = userID;  
            }
            catch (Exception ex)
            {
                Console.WriteLine("AllTripsView API " + ex.Message);
            }
            return View(ResponseModel);
        }


        public async Task<IActionResult> MarkPaymentStatus([FromQuery(Name = "tripID")] int tripID, [FromQuery(Name = "userID")] int userID)
        {
            try
            {
                var strSerializedData = string.Empty;
                ServiceHelper objService = new ServiceHelper();
                string response = await objService.PostRequest(strSerializedData, ConstantValues.SetTripPaymentStatus + "?tripId=" + tripID, false, string.Empty).ConfigureAwait(true);

            }
            catch (Exception ex)
            {
                Console.WriteLine("AllTripsView API " + ex.Message);
            }


            List<GetTripsResponseModel> MarkPaymentStatus = new List<GetTripsResponseModel>();


            GetTripsResponseModel ResponseModel = null;
            try
            {
                var strSerializedData = string.Empty;
                ServiceHelper objService = new ServiceHelper();
                string response = await objService.GetRequest(strSerializedData, ConstantValues.GetAllTripsByUser + "?userID=" + userID, false, string.Empty).ConfigureAwait(true);
                ResponseModel = JsonConvert.DeserializeObject<GetTripsResponseModel>(response);
                ResponseModel.userId = userID;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AllTripsView API " + ex.Message);
            }
            return View("Views/User/Trips.cshtml", ResponseModel);
        }



    }//end of class
}//end of namespace
