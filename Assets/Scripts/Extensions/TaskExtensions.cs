using System.Threading.Tasks;
using UnityEngine;

namespace Extensions
{
    public static class TaskExtensions
    {
        public static async Task Delay(float seconds)
        {
            if (seconds <= 0)
            {
                return;
            }
            
            var start = Time.time;
            while (Time.time - start < seconds)
            {
                await Task.Yield();
            }
        }
        
        public static async Task Delay(double seconds)
        {
            if (seconds <= 0)
            {
                return;
            }
            
            var start = Time.time;
            while (Time.time - start < seconds)
            {
                await Task.Yield();
            }
        }
    }
}