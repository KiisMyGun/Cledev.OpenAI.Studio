using Cledev.OpenAI.V1.Contracts.Images;
using Cledev.OpenAI.V1.Helpers;

using Microsoft.AspNetCore.Components.Forms;

namespace Cledev.OpenAI.Studio.Pages;

public class CreateImageEditPage : ImagePageBase
{
    protected CreateImageEditRequest Request { get; set; } = null!;

    public async Task OnInputFileForImageChange(InputFileChangeEventArgs e)
    {
        Request.Image = await GetFileBytes(e);
        Request.ImageName = e.File.Name;
    }

    public async Task OnInputFileForMaskChange(InputFileChangeEventArgs e)
    {
        Request.Mask = await GetFileBytes(e);
        Request.MaskName = e.File.Name;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Request = new CreateImageEditRequest
        {
            Image = new byte[1],
            ImageName = "Something",
            Prompt = string.Empty,
            Size = ImageSize.Size512x512.ToStringSize(),
            ResponseFormat = ImageResponseFormat.B64Json.ToStringFormat(),
            N = 1
        };
    }

    protected async Task OnSubmitAsync()
    {
        IsProcessing = true;

        Response = null;
        Error = null;
        Images.Clear();

        Response = await OpenAIClient.CreateImageEdit(Request);
        Error = Response?.Error;

        if (Response is not null)
        {
            Images.AddOriginalFromBytes(Request.Image);
            Images.AddRangeFromResponse(Response, ImageType.Edited);
        }

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Image = "必需的。要编辑的图像。必须是有效的PNG文件，小于4MB，并且为正方形。若不提供遮罩，则图像必须具有透明度，透明度将用作遮罩。";//"Required. The image to edit. Must be a valid PNG file, less than 4MB, and square. If mask is not provided, image must have transparency, which will be used as the mask.";
        public static string Mask = "可选择的一个额外的图像，其完全透明的区域（例如，alpha为零的区域）指示图像应该在哪里编辑。必须是有效的PNG文件，小于4MB，并且具有与图像相同的尺寸。";//"Optional. An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.";
        public static string Prompt = "必需的。所需图像的文本描述。最大长度为1000个字符。";//"Required. A text description of the desired image(s). The maximum length is 1000 characters.";
    }
}