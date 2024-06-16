using Microsoft.AspNetCore.Mvc;
using MegaTravelClient.Models;
using MegaTravelClient.Utility;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace MegaTravelClient.Controllers
{
   // [Authorize]
    public class AgentController : Controller
    {
        public static List<TripData> tripList = new List<TripData>();
        public IActionResult Index(LoginResponseModel userData)
        {
            return View();
        }


        public async Task<IActionResult> AllTripsView([FromQuery(Name = "agentID")] int agentID)
        {

            List<GetTripsResponseModel> userList = new List<GetTripsResponseModel>();

            GetTripsResponseModel ResponseModel = null;
            try
            {
                var strSerializedData = string.Empty;
                ServiceHelper objService = new ServiceHelper();
                string response = await objService.GetRequest(strSerializedData, ConstantValues.GetAllTripsForAgent + "?agentID=" + agentID, false, string.Empty).ConfigureAwait(true);
                ResponseModel = JsonConvert.DeserializeObject<GetTripsResponseModel>(response);
                tripList = ResponseModel.tripList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AllTripsView API " + ex.Message);
            }
            return View(ResponseModel);
        }

        public IActionResult Download()
        {
            //init memory stream and stream writer
            var memory = new MemoryStream();
            var writer = new StreamWriter(memory, new UTF8Encoding());

            //write the label of data first
            writer.WriteLine("Trip Name,Trip Location, Start Date, End Date, Number of Childs, Number of Adults, First Name,Last Name,Email,Phone");

            //for each trip write an assortment of trip data
            foreach (var trip in tripList)
            {
                writer.WriteLine(string.Join(",", trip.TripName, trip.Location,trip.StartDate,trip.EndDate,trip.NumChildren,trip.NumAdults,
                    trip.UserInfo.FirstName,trip.UserInfo.LastName,trip.UserInfo.Email,trip.UserInfo.Phone));
            }

            //commit the data to the memory
            writer.Flush();

            //set the position to the start so it can read back from the start
            memory.Position = 0;

            return File(memory, "text/csv", "tripList.csv");
        }

    }
}
