using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;

using Quartz;
using Quartz.Impl;
using Quartz.Logging;
namespace TwilioAPIs
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<SendTextJob>()
                .WithIdentity("job1", "group1")
                .Build();

            // Trigger the job to run now, and then repeat every 24 hours
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            await Task.Delay(TimeSpan.FromSeconds(60));

            await scheduler.Shutdown();

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }

    }

    public class SendTextJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //Add more messages
            string[] msgList = {
                "Good morning gorgeous, I can’t wait to shower you with hugs and kisses.",
                "Just like how a beautiful morning is incomplete without its orange hue, " +
                "my morning coffee is incomplete without texting you. Good morning.",
                "Wake up, my sweetheart! Meet a new day, you are happy, healthy, and loved by me, life is beautiful, enjoy it!"
            };
            Random rnd = new Random();
            int luckyNum = rnd.Next(0, msgList.Length);
            string msgOfTheDay = msgList[luckyNum];

            const string accountSid = "your sid";
            const string authToken = "your authToken";

            TwilioClient.Init(accountSid, authToken);

            var message = await MessageResource.CreateAsync(
                body: msgOfTheDay,
                from: new Twilio.Types.PhoneNumber("+1225*****"),
                to: new Twilio.Types.PhoneNumber("+1409******")
            );

            //Console.WriteLine(message.Sid);
            //Console.ReadLine();
        }
    }
}
