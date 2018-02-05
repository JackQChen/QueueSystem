using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;
using System.Configuration;

namespace SoundPlayer
{
    public class Voice
    {
        public SpeechSynthesizer synth = new SpeechSynthesizer(); //语音合成对象  
        public Voice()
        {
            int type = Convert.ToInt32(ConfigurationManager.AppSettings["VoiceType"]);
            //安装的tts语音包
            if (type == 0)
            {
                synth.SelectVoice("VW Hui");
            }
            else if (type == 1)
            {
                synth.SelectVoice("ScanSoft Sin-Ji_Full_22kHz");
            }
            synth.Rate = Convert.ToInt32(ConfigurationManager.AppSettings["VoiceRate"]);
        }
        public void PlayText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                synth.SpeakAsync(text);
            }
        }
    }
}
