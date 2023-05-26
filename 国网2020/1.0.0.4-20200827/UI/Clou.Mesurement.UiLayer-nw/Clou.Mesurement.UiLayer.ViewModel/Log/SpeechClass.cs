using System.Speech.Synthesis;
using Mesurement.UiLayer.Utility.Log;

namespace Mesurement.UiLayer.ViewModel.Log
{
    public class SpeechClass
    {
        private static SpeechClass instance = null;
        public static SpeechClass Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpeechClass();
                }
                return instance;
            }
        }
        private SpeechClass()
        {
            try
            {
            speaker.SetOutputToDefaultAudioDevice();
            errorSpeaker.SetOutputToDefaultAudioDevice();
            }
            catch(System.Exception e)
            {
                LogManager.AddMessage(string.Format("语音设备异常：{0}", e.Message), EnumLogSource.用户操作日志, EnumLevel.Error);
            }
        }
        private SpeechSynthesizer speaker = new SpeechSynthesizer();
        private SpeechSynthesizer errorSpeaker = new SpeechSynthesizer();
        public void Speak(LogModel model)
        {
            try
            {
                if (model.Level == EnumLevel.ErrorSpeech)
                {
                    SpeakError(model);
                }
                else
                {
                    speaker.SpeakAsync(model.Message);
                }
            }
            catch
            { }
        }
        public void SpeakError(LogModel model)
        {
            speaker.SpeakAsyncCancelAll();
            errorSpeaker.SpeakAsync(model.Message);
        }
    }
}
