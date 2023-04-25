using Cledev.OpenAI.V1.Contracts.Audio;
using Cledev.OpenAI.V1.Helpers;

using Microsoft.AspNetCore.Components.Forms;

namespace Cledev.OpenAI.Studio.Pages;

public class AudioPage : PageComponentBase
{
    public IList<string> AudioModels { get; set; } = new List<string>();
    public IList<string> ResponseFormats { get; set; } = new List<string>();
    protected CreateAudioTranscriptionRequest Request { get; set; } = null!;
    protected CreateAudioResponse? Response { get; set; }

    public async Task OnInputFileChange(InputFileChangeEventArgs e)
    {
        Request.File = await GetFileBytes(e);
        Request.FileName = e.File.Name;
    }

    protected override void OnInitialized()
    {
        Request = new CreateAudioTranscriptionRequest
        {
            Model = AudioModel.Whisper1.ToStringModel(),
            ResponseFormat = AudioResponseFormat.VerboseJson.ToStringFormat(),
            Language = "en"
        };

        AudioModels = Enum.GetValues(typeof(AudioModel)).Cast<AudioModel>().Select(x => x.ToStringModel()).ToList();
        ResponseFormats = Enum.GetValues(typeof(AudioResponseFormat)).Cast<AudioResponseFormat>().Select(x => x.ToStringFormat()).ToList();
    }

    protected async Task OnSubmitAsync()
    {
        IsProcessing = true;

        Response = null;
        Error = null;

        Response = await OpenAIClient.CreateAudioTranscription(Request);
        Error = Response?.Error;

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Prompt = "可选择的一个可选文本，用于指导模型的风格或继续上一个音频片段。提示应与音频语言相匹配。";//"Optional. An optional text to guide the model's style or continue a previous audio segment. The prompt should match the audio language.";
        public static string File = "必需的。要转录的音频文件，格式为：mp3、mp4、mpeg、mpga、m4a、wav或webm。";//"Required. The audio file to transcribe, in one of these formats: mp3, mp4, mpeg, mpga, m4a, wav, or webm.";
    }
}