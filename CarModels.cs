using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoxChallengeCore
{
    /// <summary>
    /// {"datasetId":"0CqhW9vm1gg"}
    /// </summary>
    public class DataSet
    {
        [JsonProperty(PropertyName = "datasetId")]
        public string datasetId { get; set; }
    }

    /// <summary>
    /// {"vehicleIds":[632576549,975594584,152560935,1375363866,333793695,573728913,350603415,1182434014,1395144969]}
    /// </summary>
    public class VehicleList
    {
        [JsonProperty(PropertyName = "vehicleIds")]
        public IList<string> vehicleIds { get; set; }
    }


    // {"vehicleId":632576549,"year":2004,"make":"MINI","model":"Cooper","dealerId":1585364147}
    public class VehicleInfo
    {
        [JsonProperty(PropertyName = "vehicleId")]
        public string vehicleId { get; set; }


        [JsonProperty(PropertyName = "year")]
        public string year { get; set; }

        [JsonProperty(PropertyName = "make")]
        public string make { get; set; }


        [JsonProperty(PropertyName = "model")]
        public string model { get; set; }

        [JsonProperty(PropertyName = "dealerId")]
        public string dealerId { get; set; }
    }

    // {"dealerId":1585364147,"name":"Bob's Cars"}
    public class DealerInfo
    {
        [JsonProperty(PropertyName = "dealerId")]
        public string dealerId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }
    }


    /// <summary>
    /// Vehicle Return
    /// </summary>
    public class VehicleAnswer
    {
        [JsonProperty(PropertyName = "vehicleId")]
        public string vehicleId { get; set; }


        [JsonProperty(PropertyName = "year")]
        public string year { get; set; }

        [JsonProperty(PropertyName = "make")]
        public string make { get; set; }


        [JsonProperty(PropertyName = "model")]
        public string model { get; set; }

        public VehicleAnswer(string VehicleId, string Year, string Make, string Model)
        {
            vehicleId = VehicleId; year = Year; make = Make; model = Model;
        }
    }

    /// <summary>
    /// Dealer return
    /// </summary>
    public class DealerAnswer
    {
        [JsonProperty(PropertyName = "dealerId")]
        public string dealerId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "vehicles")]
        public IList<VehicleAnswer> vehicles { get; set; }
    }

    /// <summary>
    /// Dealer Answer Return
    /// </summary>
    public class DealerAnswerReturn
    {
        [JsonProperty(PropertyName = "dealers")]
        public IList<DealerAnswer> dealers { get; set; }
    }


}
