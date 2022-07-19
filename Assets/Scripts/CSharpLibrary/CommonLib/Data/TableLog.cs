using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftup.CommonLib.Data
{
    public class TableLog
    {
        public enum LoadStatus
        {
            Loaded,
            LoadedWithDuplicatedPK,
            Skipped,
        }
        public readonly string TableName;
        public readonly LoadStatus Status;
        public readonly IEnumerable<string> LoadedFields;
        public readonly IEnumerable<string> SkippedFields;
        public readonly IEnumerable<string> DuplicatedKeys;

        public TableLog(string name)
            : this(name, LoadStatus.Skipped)
        {
        }

        public TableLog(string name, LoadStatus status)
        {
            this.TableName = name;
            this.Status = status;
            this.LoadedFields = Enumerable.Empty<string>();
            this.SkippedFields = Enumerable.Empty<string>();
            this.DuplicatedKeys = Enumerable.Empty<string>();
        }

        public TableLog(string name, IEnumerable<string> loaded, IEnumerable<string> skipped, IEnumerable<string> duplicatedKeys)
        {
            this.TableName = name;
            this.LoadedFields = loaded;
            this.SkippedFields = skipped;
            this.DuplicatedKeys = duplicatedKeys;

            if (duplicatedKeys.Any())
                this.Status = LoadStatus.LoadedWithDuplicatedPK;
            else
                this.Status = LoadStatus.Loaded;
        }

        public string[] ToCSVLog()
        {
            var loaded = String.Join(",", this.LoadedFields);
            var skipped = String.Join(",", this.SkippedFields);

            return new string[] { this.TableName, this.Status.ToString(), String.Format("\"({0})\"", loaded), String.Format("\"({0})\"", skipped) };
        }

    }
}
