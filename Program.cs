using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;

namespace Samantha
{
    class Program
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();


        static void Main(string[] args)
        {
            #region Initializing
            bool memMessage = false;
            bool cpuMessage = false;
            bool firstCpuMessage = false;
            bool firstCpuMessage2 = false;

            Random rand = new Random();
            int oldCpuValue = 0;
            int oldMemValue = 90000;
            int probablyNotTheBestWay = 0;

            Speak("Welcome to samantha version one point one", VoiceGender.Female, 0);
            #endregion

            #region Counters
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCpuCount.NextValue();
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();
            PerformanceCounter perfUptimeCount = new PerformanceCounter("System", "System Up Time");
            perfUptimeCount.NextValue();
            PerformanceCounter perfMemCount2 = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            perfMemCount2.NextValue();
            #endregion

            #region Looping
            while (true)
            {

                #region Polling
                // Get the current performance counter values
                int currentCpuPercentage = (int)perfCpuCount.NextValue();
                int currentAvailableMemory = (int)perfMemCount.NextValue();
                int currentUsedMemory = (int)perfMemCount2.NextValue();
                #endregion

                #region Checking oldValues
                if(oldCpuValue > 80 && currentCpuPercentage < 80 && firstCpuMessage2 == true)
                {
                    Speak("Cpu has calmed down, everything is ok captain", VoiceGender.Female, 1);
                    cpuMessage = false;
                }
                if(oldMemValue < 512 && currentAvailableMemory > 512)
                {
                    Speak("You now have over 512 megabytes of ram available, everything is good captain", VoiceGender.Female, 1);
                    memMessage = false;
                }
                oldMemValue = currentAvailableMemory;
                oldCpuValue = currentCpuPercentage;
                #endregion 

                #region Visual prints
                // Every 1 second print the CPU load % to the screen
                TimeSpan currentUptime = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
                Console.WriteLine("Uptime de la machine : {0} jours {1} hr {2} min {3} sec \n",
                (int)currentUptime.TotalDays,
                (int)currentUptime.Hours,
                (int)currentUptime.Minutes,
                (int)currentUptime.Seconds);
                Console.WriteLine("RAM Disponible       : {0} MB   ", currentAvailableMemory);
                Console.WriteLine("RAM Utilisée         : {0}%     \n", currentUsedMemory);
                Console.WriteLine("Utilisation CPU      : {0}%   ", currentCpuPercentage);
                #endregion

                #region Vocal Indications

                if(currentCpuPercentage > 80 && cpuMessage == false)
                {
                    if(currentCpuPercentage == 100 && cpuMessage == false)
                    {
                        if(firstCpuMessage == true)
                        {
                            Speak("ALERT. Cpu is at 100 percent", VoiceGender.Female, 1);
                            cpuMessage = true;
                        }
                    }
                    else
                    {
                        Speak("ALERT. CPU is over 80 percent", VoiceGender.Female, 1);
                    }
                    
                }

                if(currentAvailableMemory < 512 && memMessage == false)
                {
                    string memAvailableVocalMessage = String.Format("Alert. You currently have less than 512 megabytes of ram available", currentAvailableMemory);
                    Speak(memAvailableVocalMessage, VoiceGender.Female, 1);
                    memMessage = true;
                }

                #endregion

                #region Cleaning and Sleep
                Console.SetCursorPosition(0, 0);
                Thread.Sleep(1000);
                firstCpuMessage = true;

                #region No first cpu calmed down message
                if (probablyNotTheBestWay < 2)
                {
                    probablyNotTheBestWay = probablyNotTheBestWay + 1;
                }
                if(probablyNotTheBestWay == 2)
                {
                    firstCpuMessage2 = true;
                }
                #endregion

                #endregion
            }
            #endregion

        }

        public static void Speak(string message, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(message);
        }

        public static void Speak(string message, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            Speak(message, voiceGender);
        }

    }
}
