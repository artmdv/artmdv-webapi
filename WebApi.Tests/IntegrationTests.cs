using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Controllers;
using artmdv_webapi.Areas.v2.Core;
using artmdv_webapi.Areas.v2.Infrastructure;
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
    [Ignore("not now")]
    public class IntegrationTests
    {
        private HttpClient _httpClient = new HttpClient {BaseAddress = new Uri("http://localhost:5004")};
        
        private List<string> ImagesToDelete { get; set; }
        private List<Tuple<string, string>> RevisionToDelete { get; set; }

        private ImageUploadDto _newImage = new ImageUploadDto
        {
            date = DateTime.Now.Date.ToString("O"),
            description = "integrationTestDescription",
            password = new ConfigurationManager(new LocalFile()).GetPassword(),
            tags = "integrationTestTag",
            title = "integrationTestTitle"
        };

        private const string _newImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAIAAAACDbGyAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAhSURBVBhXY1Ta6MMABYwM//8zQdkg8B8ogswHAfx8BgYAD/YEKKaxNAIAAAAASUVORK5CYII=";
        private const string _newRevisionImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAIAAAACDbGyAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAhSURBVBhXY1Ta6MMABYwM//8zQdkg8B8ogswHAfx8BgYAD/YEKKaxNAIAAAAASUVORK5CYII=";

        [SetUp]
        public void Setup()
        {
            ImagesToDelete = new List<string>();
            RevisionToDelete = new List<Tuple<string, string>>();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Test]
        public async Task GettingImageReturnsCreatedImage()
        {
            var savedImage = await CreateNewImage().ConfigureAwait(false);

            var responseImage = await GetImage(savedImage.Image.Id).ConfigureAwait(false);

            Assert.AreEqual(responseImage.Image.Title, _newImage.title);
        }

        [Test]
        public async Task GetImageTag()
        {
            var tag = Guid.NewGuid().ToString();
            var revisionDescription = "revisionDescription";

            var savedImage = await CreateNewImage(tag).ConfigureAwait(false);
            await CreateRevision(savedImage.Image.Id, revisionDescription).ConfigureAwait(false);

            var updateImageDto = new ImageUpdateDto
            {
                image = savedImage.Image,
                password = new ConfigurationManager(new LocalFile()).GetPassword(),
            };
            updateImageDto.image.Annotation = ObjectId.GenerateNewId().ToString();
            updateImageDto.image.Inverted = ObjectId.GenerateNewId().ToString();

            await UpdateImage(updateImageDto).ConfigureAwait(false);


            var image = await GetImage(savedImage.Image.Id).ConfigureAwait(false);

            var response = await _httpClient.GetAsync($"v2/Images?tag={tag}").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(GetImageTag)}. Status code: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var imageJson = new StringBuilder();
            imageJson.Append(@"[{""image"":{""id"":""");
            imageJson.Append(image.Image.Id);
            imageJson.Append(@""",""type"":0,""filename"":""");
            imageJson.Append(image.Image.Filename);
            imageJson.Append(@""",""contentId"":");
            imageJson.Append("null");
            imageJson.Append(@",""description"":""");
            imageJson.Append(image.Image.Description);
            imageJson.Append(@""",""title"":""");
            imageJson.Append(image.Image.Title);
            imageJson.Append(@""",""thumb"":{""type"":1,""filename"":""");
            imageJson.Append(image.Image.Thumb.Filename);
            imageJson.Append(@""",""contentId"":""");
            imageJson.Append(image.Image.Thumb.ContentId);
            imageJson.Append(@"""},""annotation"":""");
            imageJson.Append(image.Image.Annotation);
            imageJson.Append(@""",""inverted"":""");
            imageJson.Append(image.Image.Inverted);
            imageJson.Append(@""",""tags"":[""");
            imageJson.Append(String.Join(",", image.Image.Tags));
            imageJson.Append(@"""],""date"":""");
            imageJson.Append(image.Image.Date);
            imageJson.Append(@""",""revisions"":[{""revisionDate"":""");
            imageJson.Append(image.Image.Revisions.Single().RevisionDate.ToString("yyyy-MM-ddTHH:mm:ssK"));
            imageJson.Append(@""",""revisionId"":""");
            imageJson.Append(image.Image.Revisions.Single().RevisionId);
            imageJson.Append(@""",""filename"":""");
            imageJson.Append(image.Image.Revisions.Single().Filename);
            imageJson.Append(@""",""contentId"":");
            imageJson.Append("null");
            imageJson.Append(@",""thumb"":{""type"":1,""filename"":""");
            imageJson.Append(image.Image.Revisions.Single().Thumb.Filename);
            imageJson.Append(@""",""contentId"":""");
            imageJson.Append(image.Image.Revisions.Single().Thumb.ContentId);
            imageJson.Append(@"""},""description"":""");
            imageJson.Append(image.Image.Revisions.Single().Description);
            imageJson.Append(@"""}]},""links"":{""imageContent"":""http://localhost:5004");
            imageJson.Append(@"/Images/");
            imageJson.Append(image.Image.Filename);
            imageJson.Append(@""",""thumbnailContent"":""http://localhost:5004/v2/Images/");
            imageJson.Append(image.Image.Id);
            imageJson.Append(@"/Thumbnail"",""annotationContent"":""http://localhost:5004/v2/Images/");
            imageJson.Append(image.Image.Id);
            imageJson.Append(@"/Annotation"",""invertedContent"":""http://localhost:5004/v2/Images/");
            imageJson.Append(image.Image.Id);
            imageJson.Append(@"/Inverted"",""revisions"":[{""thumb"":""http://localhost:5004/v2/Images/Content/");
            imageJson.Append(image.Image.Revisions.Single().Thumb.ContentId);
            imageJson.Append(@""",""image"":""http://localhost:5004");
            imageJson.Append(@"/Images/");
            imageJson.Append(image.Image.Revisions.Single().Filename);
            imageJson.Append(@""",""date"":""");
            imageJson.Append(image.Image.Revisions.Single().RevisionDate.ToString("yyyy-MM-ddTHH:mm:ssK"));
            imageJson.Append(@""",""description"":""");
            imageJson.Append(image.Image.Revisions.Single().Description);
            imageJson.Append(@""",""id"":""");
            imageJson.Append(image.Image.Revisions.Single().RevisionId);
            imageJson.Append(@"""}]}}]");
            
            Assert.AreEqual(imageJson.ToString(), responseJson);
        }

        [Test]
        public async Task GetImageThumbnail()
        {
            var thumbnailBase64 = "iVBORw0KGgoAAAANSUhEUgAAAZAAAAGQCAYAAACAvzbMAAAACXBIWXMAAA7DAAAOwwHHb6hkAAAPEUlEQVR4nO3Y0U1DQRRDwSWiEuqhWSoMBfDFA+XEMK7gaXwVa/P69vF+P0KAwIjAS/0BfyNffvX8DF5hvP28CSFAgACB/yhgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQJnMwakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG7gCAECBDYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAkc2BV7rDxACBL4jcMf1GwIvGH9DwAvEHREgQICAAXEDBAgQIPA4AS8Q10aAAAECBsQNECBAgIAXiBsgQIAAgScX8BdW3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN3AEQIECGwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBI5sChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkCMECBAgcEXAgLgbAgQIELgkYEAcDgECBAgYEDdAgAABAo8T8AJxbQQIECBgQNwAAQIECHiBuAECBAgQeHIBf2HVDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIHBkU8CA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgBwhQIAAgSsCBsTdECBAgMAlAQPicAgQIEDAgLgBAgQIEHicgBeIayNAgAABA+IGCBAgQMALxA0QIECAwJML+AurbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEwBECmwIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCBqRuQAgQIDAqYEDqBoQAAQKjAgakbkAIECAwKmBA6gaEAAECowIGpG5ACBAgMCpgQOoGhAABAqMCt/oDhAABAgQ2BQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IgSMENgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFDEjdgBAgQGBUwIDUDQgBAgRGBQxI3YAQIEBgVMCA1A0IAQIERgUMSN2AECBAYFTAgNQNCAECBEYFbvUHCAECBAhsChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRuQIwQIbAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECJzNGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugE5QoDApoABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwNmMAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpG5AjBAhsChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgSObAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDAqIABqRsQAgQIjAoYkLoBIUCAwKiAAakbEAIECIwKGJC6ASFAgMCogAGpGxACBAiMChiQugEhQIDA2cwnrbwIQlFvFHkAAAAASUVORK5CYII=";
            var savedImage = await CreateNewImage().ConfigureAwait(false);

            var response = await _httpClient.GetAsync($"v2/Images/{savedImage.Image.Id}/Thumbnail").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(GetImageThumbnail)}. Status code: {response.StatusCode}");
            }

            var responseBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var base64response = Convert.ToBase64String(responseBytes);

            Assert.AreEqual(thumbnailBase64, base64response);
        }

        [Test]
        public async Task GetNewImageContentReturnsNotFound()
        {
            var savedImage = await CreateNewImage().ConfigureAwait(false);

            var response = await _httpClient.GetAsync($"v2/Images/{savedImage.Image.Id}/Content").ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task GetImageContent()
        {
            var savedImage = await CreateNewImage().ConfigureAwait(false);

            var response = await _httpClient.GetAsync($"/Images/{savedImage.Image.Filename}").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(GetImageContent)}. Status code: {response.StatusCode}");
            }

            var responseBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var base64response = Convert.ToBase64String(responseBytes);

            Assert.AreEqual(_newImageBase64, base64response);
        }

        [Test]
        public async Task GettingUpdatedImageReturnsUpdatedValues()
        {
            var savedImage = await CreateNewImage().ConfigureAwait(false);

            var update = new ImageUpdateDto
            {
                image = new ImageViewModel
                {
                    Date = DateTime.Now.ToString(),
                    Description = "newDescription",
                    Annotation = ObjectId.GenerateNewId().ToString(),
                    Id = savedImage.Image.Id,
                    Filename = "newFilename",
                    Inverted = ObjectId.GenerateNewId().ToString(),
                    Tags = new []{"newTag1"},
                    Thumb = new Thumbnail { ContentId = savedImage.Image.Thumb.ContentId.ToString(), Filename = "updateThumbFilename"},
                    Title = "updatedTitle"
                },
                password = new ConfigurationManager(new LocalFile()).GetPassword(),
            };

            await UpdateImage(update).ConfigureAwait(false);

            var updatedImage = await GetImage(savedImage.Image.Id).ConfigureAwait(false);

            Assert.That(updatedImage.Image.Date, Is.EqualTo(update.image.Date));
            Assert.That(updatedImage.Image.Description, Is.EqualTo(update.image.Description));
            Assert.That(updatedImage.Image.Annotation, Is.EqualTo(update.image.Annotation));
            Assert.That(updatedImage.Image.Filename, Is.EqualTo(update.image.Filename));
            Assert.That(updatedImage.Image.Inverted, Is.EqualTo(update.image.Inverted));
            Assert.That(updatedImage.Image.Tags.Single(), Is.EqualTo(update.image.Tags.Single()));
            Assert.That(updatedImage.Image.Thumb.ContentId, Is.EqualTo(update.image.Thumb.ContentId));
            Assert.That(updatedImage.Image.Thumb.Filename, Is.EqualTo(update.image.Thumb.Filename));
            Assert.That(updatedImage.Image.Title, Is.EqualTo(update.image.Title));
        }

        [Test]
        public async Task GetFeaturedImageReturnsTheOneThatWasSet()
        {
            var password = new ConfigurationManager(new LocalFile()).GetPassword();
            var imageId = ObjectId.GenerateNewId();
            var postResponse = await _httpClient.PostAsync($"v2/Images/Featured/{imageId}/{password}", new StringContent("")).ConfigureAwait(false);

            var getResponse = await _httpClient.GetAsync($"v2/Images/Featured").ConfigureAwait(false);
            var json = await getResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var featuredImage = JsonConvert.DeserializeObject<FeaturedImageViewModel>(json);
            Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(featuredImage.ImageId, Is.EqualTo(imageId.ToString()));
        }

        [Test]
        public async Task RevisionIsRemovedFromImageAfterDeleting()
        {
            var revisionDescription = "test revision description";
            var image = await CreateNewImage().ConfigureAwait(false);
            var revision = await CreateRevision(image.Image.Id, revisionDescription).ConfigureAwait(false);
            var imageWithRevision = await GetImage(image.Image.Id).ConfigureAwait(false);
            Assert.That(imageWithRevision.Image.Revisions.Count, Is.EqualTo(1));
            Assert.That(imageWithRevision.Image.Revisions.Single().Description, Is.EqualTo(revisionDescription));
        }

        //HELPERS
        
        //IMAGE

        private async Task<ImageResponse> CreateNewImage(string tags="image", string imageBase64= _newImageBase64)
        {
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(Convert.FromBase64String(imageBase64)), "file", "file.png");
            content.Add(new StringContent(_newImage.title), "title");
            content.Add(new StringContent(_newImage.description ?? string.Empty), "description");
            content.Add(new StringContent(tags), "tags");
            content.Add(new StringContent(_newImage.date ?? string.Empty), "date");
            content.Add(new StringContent(_newImage.annotation ?? string.Empty), "annotation");
            content.Add(new StringContent(_newImage.inverted ?? string.Empty), "inverted");
            content.Add(new StringContent(new ConfigurationManager(new LocalFile()).GetPassword()), "password");
            var response = await _httpClient.PostAsync("v2/Images", content).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(CreateNewImage)}. Status code: {response.StatusCode}");
            }

            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var savedImage = JsonConvert.DeserializeObject<ImageResponse>(stringResponse);
            ImagesToDelete.Add(savedImage.Image.Id);
            return savedImage;
        }

        private async Task<ImageResponse> GetImage(string imageId)
        {
            var response = await _httpClient.GetAsync($"v2/Images/{imageId}").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(GetImage)}. Status code: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseImage = JsonConvert.DeserializeObject<ImageResponse>(responseJson);
            return responseImage;
        }

        private async Task<ImageResponse> UpdateImage(ImageUpdateDto image)
        {
            var content = new StringContent(JsonConvert.SerializeObject(image), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("v2/Images", content).ConfigureAwait(false);
            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(UpdateImage)}. Status code: {response.StatusCode}. Error: {stringResponse}");
            }
            
            return JsonConvert.DeserializeObject<ImageResponse>(stringResponse);
        }

        private async Task DeleteImage(string id)
        {
            var response = await _httpClient.DeleteAsync($"v2/Images/{id}/{new ConfigurationManager(new LocalFile()).GetPassword()}").ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(DeleteImage)}. Status code: {response.StatusCode}");
            }
        }

        //REVISIONS

        private async Task<Revision> CreateRevision(string id, string description = "", string base64image = _newRevisionImageBase64)
        {
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(Convert.FromBase64String(base64image)), "file", "file.png");
            content.Add(new StringContent(description), "description");
            content.Add(new StringContent(id), "imageId");
            content.Add(new StringContent(new ConfigurationManager(new LocalFile()).GetPassword()), "password");
            var response = await _httpClient.PostAsync("v2/Images/Revision", content).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(CreateRevision)}. Status code: {response.StatusCode}");
            }

            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var image = JsonConvert.DeserializeObject<ImageResponse>(stringResponse);
            var revision = image.Image.Revisions.OrderByDescending(x => x.RevisionDate).First();
            RevisionToDelete.Add(new Tuple<string,string>(id,revision.RevisionId));
            return revision;
        }

        private async Task DeleteRevision(Tuple<string, string> revision)
        {
            var response = await _httpClient.DeleteAsync($"v2/Images/{revision.Item1}/revision/{revision.Item2}/{new ConfigurationManager(new LocalFile()).GetPassword()}").ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error in {nameof(DeleteRevision)}. Status code: {response.StatusCode}");
            }
        }

        [TearDown]
        public async Task Teardown()
        {
            foreach (var id in RevisionToDelete)
            {
                await DeleteRevision(id).ConfigureAwait(false);
            }

            foreach (var id in ImagesToDelete)
            {
                await DeleteImage(id).ConfigureAwait(false);
            }
        }
    }
}
