using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace SpeechToText
{
    class Program
    {
        public static async Task RecognizeSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription("6e793653541a45d2b4363d2e36df1deb", "eastasia");
            var sourceLanguageConfig = SourceLanguageConfig.FromLanguage("zh-CN");
            //创建一个异步任务数组
            var stopRecognition = new TaskCompletionSource<int>();
            using (var recognizer = new SpeechRecognizer(config, sourceLanguageConfig))

            {
                recognizer.Recognizing += (s, e) =>
                {
                    //Console.WriteLine($"识别中:{e.Result.Text}");
                };
                // 识别完成后 （整段语音识别完成后会执行一次）
                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech) //识别成功
                    {
                        Console.WriteLine($"识别完成: {e.Result.Text}");
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)//未识别到语音
                    {
                        Console.WriteLine($"没有识别到语音");
                    }
                };
                //识别取消时执行
                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"取消识别: Reason={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }

                };
                //开始时执行
                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\n开始识别.");
                };
                //结束时执行
                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\n识别结束.");
                    stopRecognition.TrySetResult(0); //结束时添加一个异步任务
                };

                // 开始连续识别
                await recognizer.StartContinuousRecognitionAsync();

                //保证至少一个任务完成（等待到结束时间执行后再结束）
                Task.WaitAny(new[] { stopRecognition.Task });
            }
        }

        static void Main(string[] args)
        {
            RecognizeSpeechAsync().Wait();
        }
    }
}
