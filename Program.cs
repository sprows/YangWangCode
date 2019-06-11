using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace CoxChallengeCore
{
    class Program
    {
        static List<Task> arrayRunningTask = new List<Task>();
        static ArrayList vehicleDetailList = new ArrayList();
        static Dictionary<string, string> dealerDic = new Dictionary<string, string>();
        static Dictionary<string, DealerAnswer> dealerDetailDic = new Dictionary<string, DealerAnswer>();
        static string UserID = "";

        static void Main(string[] args)
        {
            //Stopwatch watch = Stopwatch.StartNew();

            // Start Process In Parallel
            StartProcessInParallel().Wait();

            PostAnswer();

            //Console.WriteLine($"Total process time: {watch.Elapsed}");

            DisplayData();

            Console.ReadLine();
        }

        /// <summary>
        /// Start Process In Parallel
        /// </summary>
        /// <returns></returns>
        static async Task<int> StartProcessInParallel()
        {
            DateTime ts = DateTime.Now;
            //Console.WriteLine("Start {0}", ts);

            // 0. Get User ID
            string retData = HttpUtils.GetData(HttpUtils.RESTSERVICEURI + "datasetId");
            DataSet retUser = JsonConvert.DeserializeObject<DataSet>(retData);
            UserID = retUser.datasetId;
            // Console.WriteLine("datasetId: {0}", retObj.datasetId);

            // 1. Get Vehicle List for user
            retData = HttpUtils.GetData(HttpUtils.RESTSERVICEURI + UserID + "/vehicles");

            VehicleList retVehicle = JsonConvert.DeserializeObject<VehicleList>(retData);

            foreach (string vehicleID in retVehicle.vehicleIds)
            {
                // 2. get Vehicle info in parallel 
                string Url = HttpUtils.RESTSERVICEURI + UserID + "/vehicles/" + vehicleID;
                Task<int> getVehicleTask = Task.Run(() => GetVehicle(Url));
                arrayRunningTask.Add(getVehicleTask);
                //Console.WriteLine("Start getVehicleTask : " + vehicleID);
            }

            // waiting all tasks to complete
            while (arrayRunningTask.Count > 0)
            {
                Task finished = await Task.WhenAny(arrayRunningTask);
                foreach (Task task in arrayRunningTask)
                {
                    if (task == finished)
                    {
                        arrayRunningTask.Remove(task);
                        break;
                    }
                }
            }
            return 1;
        }

        /// <summary>
        /// Get Vehicle 
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns></returns>
        static public int GetVehicle(string Uri)
        {
            //Console.WriteLine("Start Get Vehicle: " + Uri);
            string data = HttpUtils.GetData(Uri);
            vehicleDetailList.Add(data);

            VehicleInfo vehicleInfo = JsonConvert.DeserializeObject<VehicleInfo>(data);
            string del = vehicleInfo.dealerId;

            if (!dealerDic.ContainsKey(del))
            {
                dealerDic.Add(del, del);
                // 3. get dealerid from Vehicle
                string Url = HttpUtils.RESTSERVICEURI + UserID + "/dealers/" + del;

                Task<int> getVehicleTask = Task.Run(() => GetDealer(Url));
                arrayRunningTask.Add(getVehicleTask);
            }

            //Console.WriteLine("End of GetVehicle {0} \n\n{1}", Uri, data);
            return 1;
        }

        /// <summary>
        /// Get Dealer
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns></returns>
        static public int GetDealer(string Uri)
        {
            //Console.WriteLine("Start GetDealer: " + Uri);
            string data = HttpUtils.GetData(Uri);
            DealerInfo DealerInfoObj = JsonConvert.DeserializeObject<DealerInfo>(data);

            DealerAnswer DealerAnswerObj = new DealerAnswer();
            DealerAnswerObj.name = DealerInfoObj.name;
            DealerAnswerObj.dealerId = DealerInfoObj.dealerId;
            DealerAnswerObj.vehicles = new List<VehicleAnswer>();

            dealerDetailDic.Add(DealerInfoObj.dealerId, DealerAnswerObj);

            //Console.WriteLine("End of GetDealer {0} \n\n{1}", Uri, data);
            return 1;
        }


        public static string PostAnswer(string data, string id)
        {
            string Url = HttpUtils.RESTSERVICEURI + id + "/answer";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }

        /// <summary>
        /// Post Answer
        /// </summary>
        public static void PostAnswer()
        {
            foreach (string vehicle in vehicleDetailList)
            {
                VehicleInfo vehicleInfo = JsonConvert.DeserializeObject<VehicleInfo>(vehicle);
                string del = vehicleInfo.dealerId;

                if (dealerDetailDic.ContainsKey(del))
                {
                    DealerAnswer da;
                    if (dealerDetailDic.TryGetValue(del, out da))
                    {
                        VehicleAnswer vehicleAnswer = new VehicleAnswer(vehicleInfo.vehicleId,
                            vehicleInfo.year, vehicleInfo.make, vehicleInfo.model);
                        da.vehicles.Add(vehicleAnswer);
                    }
                }
                else
                {
                    Console.WriteLine("datasetId: {0} not found ", vehicleInfo.dealerId);
                }
            }

            //Console.WriteLine("datasetId: {0}", del.vehicleIds);
            DealerAnswerReturn dealerAnswer = new DealerAnswerReturn();
            dealerAnswer.dealers = dealerDetailDic.Values.ToList<DealerAnswer>(); // dic.Keys() ;

            string ret = PostAnswer(JsonConvert.SerializeObject(dealerAnswer), UserID);

            Console.WriteLine("\n\nPost request:\n" + JsonConvert.SerializeObject(dealerAnswer));
            Console.WriteLine("\n\nPost response:\n" + ret);

        }

        public static void DisplayData()
        {
            // show data
            Console.WriteLine("\n\nEnd of processing. datasetId: {0}", UserID);

            foreach (KeyValuePair<string, DealerAnswer> pair in dealerDetailDic)
            {
                string key = pair.Key;
                DealerAnswer del = pair.Value;
                Console.WriteLine("Dealer: {0} {1}", del.dealerId, del.name);
                foreach (VehicleAnswer va in del.vehicles)
                {
                    Console.WriteLine("\tVehicle: {0} {1} {2} {3}", va.make, va.vehicleId, va.model, va.year);
                }
            }

        }

    }
}
