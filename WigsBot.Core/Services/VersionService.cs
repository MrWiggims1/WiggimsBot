using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static WigsBot.Core.Services.VersionService;

namespace WigsBot.Core.Services
{
    public interface IVersionService
    {
        /// <summary>
        /// Creats a new version file and sets the correct date for the initial creation.
        /// </summary>
        /// <param name="versionNumber">The number of the version being saved, this will also be the file name.</param>
        /// <param name="versionName">The title of this verion, or the main purpose of the update.</param>
        /// <returns></returns>
        Task CreateNewVersionJson(string versionNumber, string versionName);

        /// <summary>
        /// Updates a verion with a patchnote.
        /// </summary>
        /// <param name="versionNumber">The number of the version being saved, this will also be the file name.</param>
        /// <param name="patchNote">The patch note to add to the version.</param>
        /// <param name="isMinorOrBugfix">Is this patchnote minor or small?</param>
        /// <param name="finalNote">Set this verion as complete and update the Release date?</param>
        /// <returns></returns>
        Task AddPatchNoteToVersion(string versionNumber, string patchNote, bool isMinorOrBugfix, bool finalNote);

        Task<VersionJson> ReadJson(string versionNumber);
    }

    public class VersionService : IVersionService
    {

        public async Task CreateNewVersionJson(string versionNumber, string versionName)
        {
            if (File.Exists($"/Resources/VerisionJSONs/{ versionNumber }.json"))
            {
                throw new Exception("This verion already exists.");
            }

            var json = new VersionJson
            {
                Version = versionNumber,
                Name = versionName,
                ReleaseDate = new ReleaseDate
                { Started = DateTime.Now, Release = DateTime.Now },
                PatchNotes = "empty",
                MinorNotes = "empty"
            };

            await SaveJson(json, versionNumber);
        }

        public async Task AddPatchNoteToVersion(string versionNumber, string patchNote, bool isMinorOrBugfix, bool finalNote)
        {
            var json = await ReadJson(versionNumber);

            if (isMinorOrBugfix)
            {
                if (json.MinorNotes == "empty")
                    json.MinorNotes = patchNote;
                else
                    json.MinorNotes = json.MinorNotes + "\n" + patchNote;
            }

            else
            {
                if (json.PatchNotes == "empty")
                    json.PatchNotes = patchNote;
                else
                    json.PatchNotes = json.PatchNotes + "\n" + patchNote;

            }

            if (finalNote)
                json.ReleaseDate.Release = DateTime.Now;

            await SaveJson(json, versionNumber);
        }

        public async Task SaveJson(VersionJson json, string versionNumber)
        {
            string jsonString = JsonConvert.SerializeObject(json, Formatting.Indented);

            File.WriteAllText($"/Resources/VerisionJSONs/{ versionNumber }.json", jsonString);
        }

        public async Task<VersionJson> ReadJson(string versionNumber)
        {
            if (!File.Exists($"/Resources/VerisionJSONs/{ versionNumber }.json"))
            {
                throw new FileNotFoundException("This version does not yet exist."); ;
            }

            var json = string.Empty;

            using (var fs = File.OpenRead($"/Resources/VerisionJSONs/{ versionNumber }.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            VersionJson Json = JsonConvert.DeserializeObject<VersionJson>(json);

            return Json;
        }

        public class VersionJson
        {
            public string Version { get; set; }
            public string Name { get; set; }
            public ReleaseDate ReleaseDate { get; set; }
            public string PatchNotes { get; set; }
            public string MinorNotes { get; set; }
        }

        public class ReleaseDate
        {
            public DateTime Started { get; set; }
            public DateTime Release { get; set; }
        }
    }
}
