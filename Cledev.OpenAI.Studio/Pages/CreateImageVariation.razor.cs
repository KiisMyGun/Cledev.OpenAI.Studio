﻿using Cledev.OpenAI.V1.Contracts.Images;
using Cledev.OpenAI.V1.Helpers;

using Microsoft.AspNetCore.Components.Forms;

namespace Cledev.OpenAI.Studio.Pages;

public class CreateImageVariationPage : ImagePageBase
{
    protected CreateImageVariationRequest Request { get; set; } = null!;

    public async Task OnInputFileForImageChange(InputFileChangeEventArgs e)
    {
        Request.Image = await GetFileBytes(e);
        Request.ImageName = e.File.Name;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Request = new CreateImageVariationRequest
        {
            Image = new byte[1],
            ImageName = "Something",
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

        Response = await OpenAIClient.CreateImageVariation(Request);
        Error = Response?.Error;

        if (Response is not null)
        {
            Images.AddOriginalFromBytes(Request.Image);
            Images.AddRangeFromResponse(Response, ImageType.Variation);
        }

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Image = "必须的。要用作变体基础的图像。必须是有效的PNG文件，小于4MB，并且为正方形。";//"Required. The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.";
    }
}