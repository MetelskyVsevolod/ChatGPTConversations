using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Helpers;
using UnityEngine;
using UnityEngine.Networking;
using TaskExtensions = Extensions.TaskExtensions;

namespace ChatGPT
{
    public class RequestsHandler
    {
        public const int TIMEOUT = 360;
        
        private void SetHeaders(ref UnityWebRequest req, List<(string, string)> headers)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                req.SetRequestHeader(headers[i].Item1, headers[i].Item2);
            }
        }

        public async void GetRequest<T>(string uri, System.Action<T> callback, List<(string, string)> headers = null)
        {
            var webRequest = new UnityWebRequest(uri, "GET");
            webRequest.timeout = 30;
            if (headers != null)
            {
                SetHeaders(ref webRequest, headers);
            }
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;
            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                await Task.Yield();
            }
            
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var responseJson = JsonHelper.DeserializeObject<T>(webRequest.downloadHandler.text);
                    callback(responseJson);
                    break;
            }
            
            webRequest.Dispose();
        }

        private DateTime _rateLimitResetTime;
        private static int _tokensUsed;

        private const int RateLimitMaxTokensPerMinute = 90000;
        private const int RateLimitBufferTokens = 300;

        public async Task<(T, string)> PostRequest<T>(string uri, string json, List<(string, string)> headers = null)
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                if (currentTime > _rateLimitResetTime)
                {
                    _tokensUsed = 0;
                    _rateLimitResetTime = currentTime.AddMinutes(1);
                }

                int tokensInRequest = EstimateTokenCount(json);

                if (_tokensUsed + tokensInRequest > RateLimitMaxTokensPerMinute - RateLimitBufferTokens)
                {
                    var delaySeconds = (float)(_rateLimitResetTime - currentTime).TotalSeconds;
                    await TaskExtensions.Delay(delaySeconds);
                    return await PostRequest<T>(uri, json, headers);
                }

                _tokensUsed += tokensInRequest;

                var webRequest = new UnityWebRequest(uri, "POST");

                if (headers != null)
                {
                    SetHeaders(ref webRequest, headers);
                }

                var jsonToSend = System.Text.Encoding.UTF8.GetBytes(json);
                webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.disposeDownloadHandlerOnDispose = true;
                webRequest.disposeUploadHandlerOnDispose = true;
                webRequest.timeout = TIMEOUT;
                webRequest.SendWebRequest();

                while (!webRequest.isDone)
                {
                    await Task.Yield();
                }

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    var responseJson = JsonHelper.DeserializeObject<T>(webRequest.downloadHandler.text);
                    webRequest.Dispose();
                    return (responseJson, null);
                }

                if (webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    if (uri.EndsWith("/completions"))
                    {
                        var resultError = JsonHelper.DeserializeObject<ChatGPTResultError>(webRequest.downloadHandler.text);
                        CheckForRateLimitReachedError(resultError.error.message);
                        Debug.LogError($"The following error occured: {resultError.error.message}\nWhile sending the following json:\n{json}");
                        webRequest.Dispose();
                        return (default, resultError.error.message);
                    }
                }
                else
                {
                    var errorMessage = webRequest.error;
                    Debug.LogError($"The following error occured: {webRequest.error}\nWhile sending the following json:\n{json}");
                    CheckForRateLimitReachedError(errorMessage);
                    webRequest.Dispose();
                    return (default, errorMessage);
                }

                webRequest.Dispose();
                return default;
            }
            catch (Exception exception)
            {
                DebugHelper.LogException($"{nameof(RequestsHandler)}.{nameof(PostRequest)}", exception);
            }

            return default;
        }

        private void CheckForRateLimitReachedError(string errorMessage)
        {
            var rateLimitReached = errorMessage.Contains("Rate limit reached", StringComparison.OrdinalIgnoreCase);

            if (rateLimitReached)
            {
                _rateLimitResetTime = DateTime.Now.AddMinutes(1);
            }
        }

        private int EstimateTokenCount(string text)
        {
            var averageWordLength = 5;
            var averageSentenceLength = 15;
            var averageParagraphLength = 100;

            var tokenCount = CountApproximateTokens(text, averageWordLength, averageSentenceLength, averageParagraphLength);
            return tokenCount;
        }

        private int CountApproximateTokens(string text, int averageWordLength, int averageSentenceLength, int averageParagraphLength)
        {
            var wordCount = CountWords(text);
            var sentenceCount = CountSentences(text);
            var paragraphCount = CountParagraphs(text);

            var tokenCount = wordCount * averageWordLength +
                             sentenceCount * averageSentenceLength +
                             paragraphCount * averageParagraphLength;
            return tokenCount;
        }

        private int CountWords(string text)
        {
            var words = text.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        private int CountSentences(string text)
        {
            var sentences = text.Split(new[] { '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);
            return sentences.Length;
        }

        private int CountParagraphs(string text)
        {
            var paragraphs = text.Split(new[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return paragraphs.Length;
        }
    }
}