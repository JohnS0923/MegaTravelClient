﻿
namespace MegaTravelClient.Models
{
    public class GetTripsResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<TripData> tripList { get; set; }
        public User userData { get; set; }
        public int userId { get; set; }


    }
}
