using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Controllers;
using artmdv_webapi.Areas.v2.Models;
using Microsoft.AspNetCore.Http.Internal;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace WebApi.Tests
{
    [TestFixture, Explicit]
    public class IntegrationTests
    {
        private HttpClient _httpClient = new HttpClient {BaseAddress = new Uri("http://localhost:5004")};
        

        [Test]
        public async Task GetImage()
        {
            var imageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAIAAAACDbGyAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAhSURBVBhXY1Ta6MMABYwM//8zQdkg8B8ogswHAfx8BgYAD/YEKKaxNAIAAAAASUVORK5CYII=";

            var image = new ImageUploadDto
            {
                date = DateTime.Now.Date.ToString(),
                description = "integrationTestDescription",
                password = GetPasswordFromConfig(),
                tags = "integrationTestTag",
                title = "integrationTestTitle"
            };
            var savedImage = await UploadNewImage(image.title, image.description, image.tags, image.date, image.annotation, image.inverted, image.password, imageBase64).ConfigureAwait(false);
            
            var response = await _httpClient.GetAsync($"v2/Images/{savedImage.Image.Id}").ConfigureAwait(false);
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseImage = JsonConvert.DeserializeObject<ImageResponse>(responseJson);

            Assert.AreEqual(responseImage.Image.Title, image.title);
            
            await DeleteImage(responseImage.Image.Id).ConfigureAwait(false);
        }

        [Test]
        public async Task GetImageTag()
        {
            var tag = "image";

            var imageJson = @"[{""image"":{""id"":""594184f5296ae125f09b026c"",""type"":0,""filename"":""testImage.png"",""contentId"":""594184f5296ae125f09b0268"",""description"":""Image test description"",""title"":""Image test"",""thumb"":{""type"":1,""filename"":""thumb_testImage.png"",""contentId"":""594184f5296ae125f09b026a""},""annotation"":""594184c9296ae125f09b0262"",""inverted"":""594184dc296ae125f09b0267"",""tags"":[""image""],""date"":""2017-06-14"",""revisions"":[{""revisionDate"":""2017-06-14T18:48:39.238Z"",""revisionId"":""3c24d5b7-3ff1-4e4c-864b-d297012034d6"",""filename"":""testImageRevision.png"",""contentId"":""59418507296ae125f09b026f"",""thumb"":{""type"":1,""filename"":""thumb_testImageRevision.png"",""contentId"":""59418507296ae125f09b026d""},""description"":""Revision test""}]},""links"":{""imageContent"":""http://localhost:5004?tag=image/Images/594184f5296ae125f09b026c.png"",""thumbnailContent"":""http://localhost:5004/v2/Images/594184f5296ae125f09b026c/Thumbnail"",""annotationContent"":""http://localhost:5004?tag=image/Images/594184c9296ae125f09b0262.png"",""invertedContent"":""http://localhost:5004?tag=image/Images/594184dc296ae125f09b0267.png"",""revisions"":[{""thumb"":""http://localhost:5004/v2/Images/Content/59418507296ae125f09b026d"",""image"":""http://localhost:5004?tag=image/Images/59418507296ae125f09b026f.png"",""date"":""2017-06-14 18:48:39"",""description"":""Revision test"",""id"":""3c24d5b7-3ff1-4e4c-864b-d297012034d6""}]},""forumPost"":""[url=http://localhost:5004/v2/Images/594184f5296ae125f09b026c/Content][img]http://localhost:5004/v2/Images/594184f5296ae125f09b026c/Thumbnail[/img][/url]""}]";
            
            var response = await _httpClient.GetAsync($"v2/Images?tag={tag}").ConfigureAwait(false);
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.AreEqual(imageJson, responseJson);
        }

        [Test]
        public async Task GetImageContent()
        {
            var imageId = "594184f5296ae125f09b026c";
            var imageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAZAAAAGQCAYAAACAvzbMAAAACXBIWXMAAA7DAAAOwwHHb6hkAAAFtUlEQVR4nO3XsQ2DUBAFQUBU4nrcrCuEAoi8yYmvmQpedKs7P7/vtQEvsU8PWMPj6jmDxTE9AIB3EhAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIBEQABIBASAREAASAQEgERAAEgEBIDmnBwD/uKYHrGGfHrAGHwgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICACJgACQCAgAiYAAkAgIAImAAJAICADJDa28CEJQqEfiAAAAAElFTkSuQmCC";
            
            var response = await _httpClient.GetAsync($"v2/Images/{imageId}/Thumbnail").ConfigureAwait(false);
            var responseBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var base64response = Convert.ToBase64String(responseBytes);

            Assert.AreEqual(imageBase64, base64response);
        }

        [Test]
        public async Task GetImageThumbnail()
        {
            var imageId = "594184f5296ae125f09b026c";
            var imageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAIAAAACDbGyAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAhSURBVBhXY1Ta6MMABYwM//8zQdkg8B8ogswHAfx8BgYAD/YEKKaxNAIAAAAASUVORK5CYII=";
            
            var response = await _httpClient.GetAsync($"v2/Images/{imageId}/Content").ConfigureAwait(false);
            var responseBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var base64response = Convert.ToBase64String(responseBytes);

            Assert.AreEqual(imageBase64, base64response);
        }

        [Test]
        public async Task GetContentById()
        {
            var imageId = "59418507296ae125f09b026d";
            var imageBase64 = "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m8dtn+bdC137vH57qW375v12TftB3pxvz+k+bP868FkCbEMFAkCk+BdKaaID1Mk03sFND/D5AAQsJBxiISFAK7cyASBS9zoQN1MEKKZ4D8Tt3AHqVPxWJtsf2hdKASxBEx2AyIgm+j4v7proAGUV74FsYq11IB3H1e8AjqBsD+T0pE10gAOouwdiGi9AMcXvhbX9oSf+e7Zaid48/lWgAfvmbrwARC6nps7fdM3CAihLE33kSVhICPBW3U10AIqp+2aKY/k+EIDF1b2QsLZpvE3X04r/D4DDcjNFACK+UAqAiFlYa/B9IMABmIUFQKTuJnpp7sYLHJhbmQAQKd4D2b2tbqYIUBkB8oVbmQBMVneA1LaQEOCJWAcCQGTENF4XYwDeO19fTtfSJwFAfUzjBSAiQACICBAAIgIEgIgAASAiQACICBAAIgIEgIgAASAiQACICBAAIgIEgMiHr/dFL/UAoFoqEAAiZQNEBQNQLRUIAJG6A0SlAlBM3QECQDECBIDIMQLEEBfA4o4RIAAsToAAEBEgAEQECAARAQJARIAAEBEgAEQECAARAfKFRYYAkwkQACKX0iewC6+lTwCgPioQACICBICIAAEgIkAAiAgQACICBICIAAEgIkAAiAgQACICBIDIMW5l4lYlAItTgQAQESAARAQIAJG6eyB6GwDFqEAAiAgQACJlh7AMQQFUSwUCQESAABA5X19O19InAUB9VCAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkBEgAAQESAARAQIABEBAkDk/6l5FqB6eYsyAAAAAElFTkSuQmCC";
            
            var response = await _httpClient.GetAsync($"v2/Images/Content/{imageId}").ConfigureAwait(false);
            var responseBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var base64response = Convert.ToBase64String(responseBytes);

            Assert.AreEqual(imageBase64, base64response);
        }

        [Test]
        public async Task GetFeaturedImageReturnsTheOneThatWasSet()
        {
            var password = GetPasswordFromConfig();
            var imageId = ObjectId.GenerateNewId();
            var postResponse = await _httpClient.PostAsync($"v2/Images/Featured/{imageId}", new StringContent("{\"password\":\"" + password + "\"}")).ConfigureAwait(false);


            var getResponse = await _httpClient.GetAsync($"v2/Images/Featured").ConfigureAwait(false);
            var json = await getResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var featuredImage = JsonConvert.DeserializeObject<FeaturedImage>(json);
            Assert.That(postResponse.IsSuccessStatusCode);
            Assert.That(getResponse.IsSuccessStatusCode);
            Assert.That(featuredImage.ImageId, Is.EqualTo(imageId));
        }

        private async Task<ImageResponse> UploadNewImage(string title, string description,string tags, string date, string annotation, string inverted, string password, string file)
        {
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(Convert.FromBase64String(file)), "file", "file.jpg");
            content.Add(new StringContent(title), "title");
            content.Add(new StringContent(description ?? string.Empty), "description");
            content.Add(new StringContent(tags), "tags");
            content.Add(new StringContent(date ?? string.Empty), "date");
            content.Add(new StringContent(annotation ?? string.Empty), "annotation");
            content.Add(new StringContent(inverted ?? string.Empty), "inverted");
            content.Add(new StringContent(password), "password");
            var response = await _httpClient.PostAsync("v2/Images", content).ConfigureAwait(false);
            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ImageResponse>(stringResponse);
        }

        private async Task DeleteImage(string id)
        {
            await _httpClient.DeleteAsync($"v2/Images/{id}/{GetPasswordFromConfig()}").ConfigureAwait(false);
        }

        private string GetPasswordFromConfig()
        {
            var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read);
            JObject config = null;
            using (StreamReader streamReader = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                config = (JObject)JToken.ReadFrom(reader);
            }
            return config?.GetValue("password").ToString();
        }
    }
}
