using Godot;
using Rhythmer;
using Godot.Collections;

namespace Game
{
    public class SubtitleLabelClass : Label
    {
        private Array TranslationData = new Array();

        public override void _Ready()
        {
            File translated = new File();
            translated.Open("res://subtitles.json", File.ModeFlags.Read);
            string translatedJSON = translated.GetAsText();
            translated.Close();

            JSONParseResult translatedJSONParsed = JSON.Parse(translatedJSON);
            Dictionary Data = (Dictionary) translatedJSONParsed.Result;
            TranslationData = (Array) Data["zorori_linesjp"];
        }

        public void ConnectToASPTimedSingle(ASPTimedSingle @ASPTimedSingle)
        {
            ASPTimedSingle.Connect(nameof(@ASPTimedSingle.NotifiedPlayback), this, nameof(_on_ASPTimedSingle_NotifiedPlayback));
        }

        public void _on_ASPTimedSingle_NotifiedPlayback(bool isPlaying, string IdType, int Id)
        {
            if (IdType == "fail") Text = "";
            if (IdType != "gag") return;

            Text = isPlaying ? (string) TranslationData[Id] : "";
        }

        public void Clear() => Text = "";
    }
}
