
using Microsoft.AspNetCore.Mvc;
using petmanagement.models;
using System.Collections;
using System.Collections.Generic;
using MediatR;

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
//using MediatR;

using Cassandra;
using petmanagement.database;
using Google.Cloud.PubSub.V1;
using System.Linq;
using System;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;

namespace petmanagement.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        public UserProfileController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet]
        [ActionName("GetAllusers")]
        public ActionResult<userprofilecreate> GetAllusers()
        {

            var session = _dbConnection.GetCassandrasession();

            var rowSet = session.Execute("select * from petmanagement.userinfo;");
            IEnumerable<Row> rows = rowSet.GetRows();
            return Ok(rows);
        }

        [HttpGet]
        [ActionName("GetUserByID")]
        public ActionResult<userprofilecreate> GetUserByID(int id)
        {


            var session = _dbConnection.GetCassandrasession();

            var rowSet = session.Execute("select * from petmanagement.userinfo where userid = " + id + ";");

            IEnumerable<Row> rows = rowSet.GetRows();
            //if (rows.GetEnumerator().MoveNext() == true)
            //{
            //    return Ok(rows);
            //}
            return Ok(rows);// NoContent();
            
        }

        [HttpPost]
        [ActionName("Createprofile")]
        ///POST 
        public ActionResult<userprofilecreate> Createprofile(int userid ,string Name,string Address,string State, string Country, string Email, string PAN, string Phoneno)
        {

            var session = _dbConnection.GetCassandrasession();

            var rowSet = session.Execute("Insert into petmanagement.userinfo(userid, Name, Address, State, Country, Email, PAN, Phoneno) values (" + userid +",'" + Name + "','" + Address + "','" + State + "','" + Country + "','" + Email + "','" + PAN + "','" + Phoneno + "');");
            IEnumerable<Row> rows = rowSet.GetRows();
            return Ok(rows);
        }

        [HttpGet]
        [ActionName("getstoredetails")]
        ///GET
        public ActionResult<storeinfo> getstoredetails(string storename)
        {

            var session = _dbConnection.GetCassandrasession();
            var rowSet = session.Execute("select * from petmanagement.storeinfo where storename = '" + storename + "';");
            IEnumerable<Row> rows = rowSet.GetRows();
            return Ok(rows);
        }

        [HttpPost]
        [ActionName("Savepetinfo")]
        ///POST 
        public ActionResult<userprofilecreate> Savepetinfo(int userid, string Name, string Address, string State, string Country, string Email, string PAN, string Phoneno)
        {

            var session = _dbConnection.GetCassandrasession();

            var rowSet = session.Execute("Insert into petmanagement.userinfo(userid, Name, Address, State, Country, Email, PAN, Phoneno) values (" + userid + ",'" + Name + "','" + Address + "','" + State + "','" + Country + "','" + Email + "','" + PAN + "','" + Phoneno + "');");
            IEnumerable<Row> rows = rowSet.GetRows();
            return Ok(rows);
        }

        [HttpPost]
        [ActionName("POSTPUBSUB")]
        public async Task<bool> postMessage([FromBody]string message)
        {

            List<string> lst = new List<string> { message };
            int i = await PublishMessagesAsync("petsmartws", "PostMessage", lst);
            if (i > 0)
                return true;
            else
                return false;

        }


        [HttpGet]
        [ActionName("GETPUBSUB")]
        public async Task<List<string>> getMessage()

        {

            return await PullMessagesAsync("petsmartws", "PostMessage-sub", false);
        }

        private async Task<int> PublishMessagesAsync(string projectId, string topicId, IEnumerable<string> messageTexts)

        {
            var credential = GoogleCredential.FromJson(@"{""petsmartws - ee41e17ee4c9.json"":""./petmanagement""}");
            var channel = new Grpc.Core.Channel(PublisherServiceApiClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
            //var publisher = PublisherServiceApiClient.Create(channel);


            TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);
            int publishedMessageCount = 0;

            var publishTasks = messageTexts.Select(async text =>
            {
                try

                {

                    string message = await publisher.PublishAsync(text);

                    Interlocked.Increment(ref publishedMessageCount);

                }

                catch (Exception exception)

                {

                }

            });

            await Task.WhenAll(publishTasks);

            return publishedMessageCount;

        }



        private async Task<List<string>> PullMessagesAsync(string projectId, string subscriptionId, bool acknowledge)

        {

            List<string> lstmsg = new List<string>();

            string text = string.Empty;

            var credential = GoogleCredential.FromJson(@"{""petsmartws - ee41e17ee4c9.json"":""./petmanagement""}");
            var channel = new Grpc.Core.Channel(PublisherServiceApiClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());


            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
            

            // SubscriberClient runs your message handle function on multiple

            // threads to maximize throughput.

            int messageCount = 0;

            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>

            {

                text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());



                Interlocked.Increment(ref messageCount);

                return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);

            });



            // Run for 5 seconds.

            await Task.Delay(5000);

            await subscriber.StopAsync(CancellationToken.None);

            // Lets make sure that the start task finished successfully after the call to stop.

            await startTask;

            if (!string.IsNullOrEmpty(text))

                lstmsg.Add(text);

            return lstmsg;

        }

    }
}