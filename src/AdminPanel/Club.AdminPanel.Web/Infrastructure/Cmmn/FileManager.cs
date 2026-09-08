#if undercheck

global using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Base;

using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.Relationship;
using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;
using Path = System.IO.Path;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.ObjectStorage;
public class FileManager(ILogger Logger) : ICmmnFileManager
{
    public string SaveFileData(string entityId, string fieldId,
        bool isPhysicalLocationDateBased, DateBasedResolution dateResolution, bool isPhysicalLocationCategoryBased,
        string category,
        string transactionId, object fileData, AuditTrail auditTrail, ref ExceptionInfos errors)
    {
        string[] arrfileData = fileData?.ToString().Split('|');
        string guid;
        if (arrfileData == null || arrfileData.Length < 2)
        {
            //fileData is guid which created last.
            guid = arrfileData?[0];
            if (string.IsNullOrEmpty(guid))
            {
                DeleteFile(auditTrail, ref errors, guid);
            }
        }
        else
        {
            string extension = Path.GetExtension(arrfileData[1])?.ToLower();
            if (ExtensionIsNotValid(extension))
            {
                return "";
            }

            guid = Guid.NewGuid().ToString();
            string fileName = Path.GetFileNameWithoutExtension(arrfileData[1]);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = guid;
            }

            string[] arr = arrfileData[0].Split(';');
            string contentType = arr[0][5..]; //"data:...;base64,.....;
            string fileInfoData = arr.Length > 1 && arr[1] != null && arr[1].Length > 7 ? arr[1][7..] : "";
            //"data:...;base64,.....;
            if (arr.Length > 2)
            {
                fileInfoData += string.Join(";", arr.Skip(2));
            }

            DateTime curTime = DateTime.UtcNow;
            string crc = GetCrc(fileInfoData);
            bool b = SaveFileInfo(guid, fileName, GetExtensionWithoutDot(extension),
                contentType, entityId, fieldId,
                isPhysicalLocationDateBased, dateResolution, isPhysicalLocationCategoryBased, category,
                fileInfoData.Length, transactionId, curTime, crc, auditTrail, ref errors);
            if (b)
            {
                if (ProjectDefinition.Project.FileMethod == FileMethod.FileSystem.ToString())
                {
                    b = SaveFileToServer(guid + extension, fileInfoData, curTime,
                        isPhysicalLocationDateBased, dateResolution, isPhysicalLocationCategoryBased, category);
                }
                else if (ProjectDefinition.Project.FileMethod == FileMethod.BinaryDbms.ToString())
                {
                    b = SaveFileToDbServer(guid, fileInfoData, auditTrail);
                }
            }

            if (!b)
            {
                guid = "";
            }
        }

        return guid;
    }

    public void DeleteFile(AuditTrail auditTrail, ref ExceptionInfos errors, string guid)
    {
        DeleteFileFromServer(guid, auditTrail);
        DeleteFileInfo(guid, auditTrail, ref errors);
    }

    public string MoveAndSaveFile(string entityId, string fieldId,
        bool isPhysicalLocationDateBased, DateBasedResolution dateResolution, bool isPhysicalLocationCategoryBased,
        string category,
        string transactionId,
        object sourceFileUuid, AuditTrail auditTrail, ref ExceptionInfos errors)
    {
        if (ProjectDefinition.Project.FileMethod == FileMethod.BinaryDbms.ToString())
        {
            throw new NotImplementedException();
        }

        string path = Path.Combine(UploadedFilesPath(), sourceFileUuid?.ToString() ?? "");
        string filePath;
        try
        {
            string[] files = Directory.GetFiles(path);
            if (files.Length != 1)
            {
                return "";
            }

            filePath = files[0];
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.Message);
            return sourceFileUuid?.ToString() ?? "";
        }

        string extension = Path.GetExtension(filePath).ToLower();
        if (ExtensionIsNotValid(extension))
        {
            return "";
        }

        string guid = Guid.NewGuid().ToString();
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        DateTime curTime = DateTime.UtcNow;
        return !SaveFileInfo(guid, fileName, GetExtensionWithoutDot(extension),
            GetMimeType(extension), entityId, fieldId,
            isPhysicalLocationDateBased, dateResolution, isPhysicalLocationCategoryBased, category,
            new FileInfo(filePath).Length, transactionId, curTime, GetCrc(""), auditTrail, ref errors)
            ? ""
            : ProjectDefinition.Project.FileMethod != FileMethod.FileSystem.ToString()
            ? throw new Exception("FileMethod not defined in project.")
            : MoveFileToServer(guid + extension, filePath, curTime, isPhysicalLocationDateBased,
              dateResolution,
              isPhysicalLocationCategoryBased, category, ref errors) ? guid : "";
    }

    private DataSource GetFileDataSource(IAuditTrail auditTrail)
    {
        return DataSourceProviderManager.GetProvider("default")
            .GetDataSource(ProjectDefinition.Project.GetEntityByEntityId("FileData"),
                [], auditTrail) as DataSource;
    }

    private bool SaveFileToDbServer(string guid, string fileInfoData, AuditTrail auditTrail)
    {
        DataSource ds = GetFileDataSource(auditTrail);
        ds.SaveFile(guid, Convert.FromBase64String(fileInfoData));
        ds.Release();
        return true;
    }

    public void DeleteFileData(string transactionId, AuditTrail auditTrail,
        ref ExceptionInfos errors)
    {
        QueryUtility fileInfo = QueryUtility<NeoFileInfo>.New();
        fileInfo.SelectFields("Id", "Time");
        fileInfo.SelectField("CategoryId")
            .Include("Category")
            .SelectFields("IsPhysicalLocationCategoryBased", "IsPhysicalLocationDateBased")
            .SelectField("Name", "CategoryName");
        fileInfo.AddFilter("TransactionId='" + transactionId + "'");
        if (fileInfo.GetDocuments())
        {
            string guid;
            do
            {
                guid = DeleteFileFromServer(fileInfo, auditTrail);
            } while (guid != null);
        }

        fileInfo.ReleaseQuery();
        ApplyUtility delFileInfo = ApplyUtility<NeoFileInfo>.NewInTrail(auditTrail);
        delFileInfo.DeleteWithFilter("TransactionId='" + transactionId + "'");
        AppendError(ref errors, delFileInfo);
        delFileInfo.Release();
    }

    private void AppendError(ref ExceptionInfos errors, ApplyUtility delFileInfo)
    {
        if (delFileInfo.Errors != null && delFileInfo.Errors.Any())
        {
            errors ??= [];
            errors.AddRange(delFileInfo.Errors);
        }
    }

    public string GetFileName(string guid)
    {
        QueryUtility fileInfo = QueryUtility<NeoFileInfo>.New();
        fileInfo.SelectFields("FileName");
        fileInfo.AddFilter("Id='" + guid + "'");
        if (!fileInfo.GetDocuments())
        {
            return null;
        }

        ElasticObject record = fileInfo.GetRecord();
        fileInfo.ReleaseQuery();
        return record == null ? null : record.GetField("FileName", out object oFileName) ? oFileName?.ToString() ?? "" : null;
    }

    public Stream GetFileStream(string guid, out string fileName, out string contentType,
        AuditTrail auditTrail)
    {
        fileName = "";
        contentType = "";
        ElasticObject record = FetchFileInfoRecord(guid);
        if (record == null)
        {
            return null;
        }

        if (!record.GetField("Time", out object otime) || otime == null)
        {
            return null;
        }

        record.GetField("Extention", out object oExt);
        record.GetField("FileName", out object oFileName);
        fileName = oFileName?.ToString() ?? "";
        fileName += oExt != null ? "." + oExt : "";
        record.GetField("ContentType", out object oContentType);
        contentType = oContentType?.ToString() ?? "";
        if (ProjectDefinition.Project.FileMethod == FileMethod.FileSystem.ToString())
        {
            ObtainRelativePathParameters(out string extension, oExt, record, out DateTime time, out bool isPhysicalLocationDateBased,
                out DateBasedResolution dateResolution, out bool isPhysicalLocationCategoryBased, out string categoryName, out string specificRoot);
            string fullFileName = GetFileFullName(guid, time, isPhysicalLocationDateBased, dateResolution,
                isPhysicalLocationCategoryBased,
                categoryName, extension ?? "", specificRoot, false);
            //ExtractRarToFile(fullFileName);
            //var extraxtFileName =
            //	Path.Combine(GetFilePath(time, isPhysicalLocationDateBased, isPhysicalLocationCategoryBased, categoryName),
            //		Path.GetFileNameWithoutExtension(fullFileName) + "."+extension);

            //                _Logger.LogDebug("Download fullFileName: {0}", fullFileName);

            //			    using (NetworkShareAccesser.LoginIfNeeded(fullFileName))
            if (File.Exists(fullFileName))
            {
                return new FileStream(fullFileName, FileMode.Open);
            }
        }
        else if (ProjectDefinition.Project.FileMethod == FileMethod.BinaryDbms.ToString())
        {
            byte[] data = null;
            data = GetFileDataFromServer(guid, auditTrail);
            return new MemoryStream(data);
        }

        return null;
    }

    public byte[] GetFileData(string guid, out string fileName, out string contentType, AuditTrail auditTrail)
    {
        fileName = "";
        contentType = "";
        ElasticObject record = FetchFileInfoRecord(guid);
        if (record == null)
        {
            return null;
        }

        if (!record.GetField("Time", out object otime) || otime == null)
        {
            return null;
        }

        byte[] data = null;
        record.GetField("Extention", out object oExt);
        record.GetField("FileName", out object oFileName);
        fileName = oFileName?.ToString() ?? "";
        fileName += oExt != null ? "." + oExt : "";
        record.GetField("ContentType", out object oContentType);
        contentType = oContentType?.ToString() ?? "";
        if (ProjectDefinition.Project.FileMethod == FileMethod.FileSystem.ToString())
        {
            ObtainRelativePathParameters(out string extension, oExt, record, out DateTime time, out bool isPhysicalLocationDateBased,
                out DateBasedResolution dateResolution, out bool isPhysicalLocationCategoryBased, out string categoryName, out string specificRoot);
            string fullFileName = GetFileFullName(guid, time, isPhysicalLocationDateBased, dateResolution,
                isPhysicalLocationCategoryBased,
                categoryName, extension ?? "", specificRoot, false);
            //ExtractRarToFile(fullFileName);
            //var extraxtFileName =
            //	Path.Combine(GetFilePath(time, isPhysicalLocationDateBased, isPhysicalLocationCategoryBased, categoryName),
            //		Path.GetFileNameWithoutExtension(fullFileName) + "."+extension);

            //                _Logger.LogDebug("Download fullFileName: {0}", fullFileName);

            //			    using (NetworkShareAccesser.LoginIfNeeded(fullFileName))

            if (File.Exists(fullFileName))
            {
                data = File.ReadAllBytes(fullFileName);
                //File.Delete(extraxtFileName);
            }

        }
        else if (ProjectDefinition.Project.FileMethod == FileMethod.BinaryDbms.ToString())
        {
            data = GetFileDataFromServer(guid, auditTrail);
        }

        return data;
    }

    public void CleanupFileSystem(string categoryName = null)
    {
        bool FileIsDefinedInDatabase(string file)
        {
            string guid = Path.GetFileNameWithoutExtension(file);
            return QueryUtility<NeoFileInfo>.New()
            .Where("Id='" + guid + "'")
            .Any();
        }

        string root = AcquireRoot(categoryName);
        foreach (string file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
        {
            if (!FileIsDefinedInDatabase(file))
            {
                File.Delete(file);
            }
        }
    }

    private void ObtainRelativePathParameters(out string extension, object oExt, ElasticObject record,
    out DateTime time, out bool isPhysicalLocationDateBased,
    out DateBasedResolution dateResolution,
    out bool isPhysicalLocationCategoryBased, out string categoryName, out string specificRoot)
    {
        extension = oExt?.ToString();
        time = record.GetDateTime("Time");
        isPhysicalLocationDateBased = true;
        dateResolution = DateBasedResolution.Monthly;
        isPhysicalLocationCategoryBased = false;
        categoryName = "";
        specificRoot = "";
        string categoryId = record.GetString("CategoryId");
        if (!string.IsNullOrEmpty(categoryId))
        {
            specificRoot = record.GetString("SpecificRoot");
            isPhysicalLocationDateBased = record.GetBool("IsPhysicalLocationDateBased", true);
            dateResolution = record.GetEnum("DateResolution", DateBasedResolution.Monthly);
            isPhysicalLocationCategoryBased = record.GetBool("IsPhysicalLocationCategoryBased", false);
            if (isPhysicalLocationCategoryBased)
            {
                categoryName = record.GetString("CategoryName");
            }
        }
    }

    private ElasticObject FetchFileInfoRecord(string guid)
    {
        QueryUtility fileInfo = QueryUtility<NeoFileInfo>.New();
        fileInfo.SelectFields("Id", "Time", "Extention", "FileName", "ContentType");
        fileInfo.AddFilter("Id='" + guid + "'");
        fileInfo.SelectField("CategoryId")
            .Include("Category")
            .SelectFields("IsPhysicalLocationCategoryBased", "IsPhysicalLocationDateBased", "DateResolution",
                "SpecificRoot")
            .SelectField("Name", "CategoryName");
        if (!fileInfo.GetDocuments())
        {
            return null;
        }

        ElasticObject record = fileInfo.GetRecord();
        fileInfo.ReleaseQuery();
        return record;
    }

    public PointableFileInfo GetPointableFileInfo(string guid)
    {
        ElasticObject record = FetchFileInfoRecord(guid);
        if (record == null)
        {
            throw new Exception("File's record Not Found");
        }

        record.GetField("Extention", out object oExt);
        ObtainRelativePathParameters(out string extension, oExt, record, out DateTime time,
            out bool isPhysicalLocationDateBased, out DateBasedResolution dateResolution, out bool isPhysicalLocationCategoryBased,
            out string categoryName, out string specificRoot);
        List<string> relativePath = [];
        if (isPhysicalLocationCategoryBased && !string.IsNullOrEmpty(categoryName))
        {
            relativePath.AddRange(categoryName.Split('.'));
        }

        if (isPhysicalLocationDateBased)
        {
            switch (dateResolution)
            {
                case DateBasedResolution.Monthly:
                    relativePath.Add(time.Year.ToString());
                    relativePath.Add(time.Month.ToString());
                    break;
                case DateBasedResolution.Daily:
                    relativePath.Add(time.Year.ToString());
                    relativePath.Add(time.Month.ToString());
                    relativePath.Add(time.Day.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dateResolution), dateResolution, null);
            }
        }

        relativePath.Add(guid + "." + extension);
        return new PointableFileInfo(record.GetString("ContentType"), relativePath, guid);
        //            throw new Exception("File category isn't well defined in database");
    }

    private byte[] GetFileDataFromServer(string guid, AuditTrail auditTrail)
    {
        DataSource ds = GetFileDataSource(auditTrail);
        ds.AddField(null/*todo*/, "Id", guid);
        ds.AddField(null/*todo*/, "Data", null);
        ds.AddFilter("Id='" + guid + "'", null, null);
        if (ds.openForRead() && ds.read(out ElasticObject record))
        {
            if (record != null)
            {
                return (byte[])record["Data"];
            }
        }

        ds.Release();
        return null;
    }

    private string GetCrc(string fileInfoData)
    {
        //todo 
        return "";
    }

    private void DeleteFileFromServer(string guid, AuditTrail auditTrail)
    {
        QueryUtility fileInfo = QueryUtility<NeoFileInfo>.New();
        fileInfo.SelectFields("Id", "Time", "Extention");
        fileInfo.AddFilter("Id='" + guid + "'");
        fileInfo.SelectField("CategoryId")
            .Include("Category")
            .SelectFields("IsPhysicalLocationCategoryBased", "DateResolution", "IsPhysicalLocationDateBased",
                "SpecificRoot")
            .SelectField("Name", "CategoryName");
        if (fileInfo.GetDocuments())
        {
            DeleteFileFromServer(fileInfo, auditTrail);
        }

        fileInfo.ReleaseQuery();
    }

    private string DeleteFileFromServer(QueryUtility fileInfo, AuditTrail auditTrail)
    {
        ElasticObject record = fileInfo.GetRecord();
        object guid = null;
        record?.GetField("Id", out guid);
        if (guid == null)
        {
            return null;
        }

        if (ProjectDefinition.Project.FileMethod == FileMethod.FileSystem.ToString())
        {
            if (record.GetField("Time", out object otime) && otime != null)
            {
                DateTime time = Convert.ToDateTime(otime);
                object extension = record["Extention"];
                bool isPhysicalLocationDateBased = true;
                bool isPhysicalLocationCategoryBased = false;
                DateBasedResolution dateResolution = DateBasedResolution.Monthly;
                string categoryName = "", specificRoot = "";
                string categoryId = record.GetString("CategoryId"); // todo what!?! 
                if (!string.IsNullOrEmpty(categoryId))
                {
                    specificRoot = record.GetString("SpecificRoot");
                    isPhysicalLocationDateBased = record.GetBool("IsPhysicalLocationDateBased", true);
                    dateResolution = record.GetEnum("DateResolution", DateBasedResolution.Monthly);
                    isPhysicalLocationCategoryBased = record.GetBool("IsPhysicalLocationCategoryBased", false);
                    if (isPhysicalLocationCategoryBased)
                    {
                        categoryName = record.GetString("CategoryName");
                    }
                }

                string fullFileName = GetFileFullName(guid.ToString(), time, isPhysicalLocationDateBased,
                    dateResolution, isPhysicalLocationCategoryBased, categoryName, extension?.ToString() ?? "",
                    specificRoot, false);
                if (!File.Exists(fullFileName))
                {
                    bool fileExists = File.Exists(fullFileName);
                    if (fileExists)
                    {
                        File.Delete(fullFileName);
                    }
                }
                else
                {
                    File.Delete(fullFileName);
                }

                return guid.ToString();
            }
        }

        if (ProjectDefinition.Project.FileMethod == FileMethod.BinaryDbms.ToString())
        {
            DeleteFileFromDbServer(guid.ToString(), auditTrail);
        }

        return null;
    }

    private void DeleteFileFromDbServer(string guid, AuditTrail auditTrail)
    {
        DataSource ds = GetFileDataSource(auditTrail);
        ds.AddFilter("Id='" + guid + "'", null, null);
        ds.DeleteGroup(new ElasticObject());
        ds.Release();
    }

    private void DeleteFileInfo(string guid, AuditTrail auditTrail, ref ExceptionInfos errors)
    {
        ApplyUtility fileInfo = ApplyUtility<NeoFileInfo>.NewInTrail(auditTrail);
        fileInfo.AddField("Id", guid);
        ElasticObject record = new();
        record.SetField("Id", guid);
        fileInfo.Delete(record, null);
        AppendError(ref errors, fileInfo);
        fileInfo.Release();
    }

    private bool SaveFileInfo(string guid, string fileName, string extension,
        string contentType, string entityId, string fieldId, bool isPhysicalLocationDateBased,
        DateBasedResolution dateResolution, bool isPhysicalLocationCategoryBased,
        string category, long size, string transactionId, DateTime curTime,
        string crc, AuditTrail auditTrail, ref ExceptionInfos errors)
    {
        ElasticObject categoryRecord = QueryUtility.New("SystemConfigs", "FileCategory").Find($"Id=='{category}'", null);
        if (categoryRecord == null)
        {
            categoryRecord = new ElasticObject
            {
                ["Id"] = category,
                ["Name"] = category,
                ["IsPhysicalLocationDateBased"] = isPhysicalLocationDateBased,
                ["IsPhysicalLocationCategoryBased"] = isPhysicalLocationCategoryBased,
                ["DateResolution"] = dateResolution
            };
            ApplyUtility catFileInfo = ApplyUtility<NeoFileCategory>.NewInTrail(auditTrail);
            catFileInfo.Insert(categoryRecord, null);
            AppendError(ref errors, catFileInfo);
            catFileInfo.Release();
        }

        ElasticObject fileInfoRecord = new()
        {
            ["Id"] = guid,
            ["FileName"] = fileName,
            ["Extention"] = extension,
            ["ContentType"] = contentType,
            ["Size"] = size,
            ["Entity"] = entityId,
            ["FieldName"] = fieldId,
            ["CategoryId"] = category,
            ["TransactionId"] = transactionId,
            ["Time"] = curTime.Year + "/" + curTime.Month + "/" + curTime.Day,
            ["CRC"] = crc
        };
        ApplyUtility fileInfo = ApplyUtility<NeoFileInfo>.NewInTrail(auditTrail);
        bool b = fileInfo.Insert(fileInfoRecord, null);
        AppendError(ref errors, fileInfo);
        fileInfo.Release();
        return b;
    }

    private bool ExtensionIsNotValid(string extension)
    {
        return extension == null || ExtensionIsNotAllowed(extension);
    }

    private bool ExtensionIsNotAllowed(string extension)
    {
        return extension is "bat" or "bin" or "cmd" or "com" or
               "cpl" or
               "exe" or "gadget" or "inf1" or "ins" or
               "inx" or
               "isu" or "job" or "jse" or "lnk" or
               "msc" or
               "msi" or "msp" or "mst" or "paf" or
               "pif" or
               "ps1" or "reg" or "rgs" or "sct" or
               "shb" or
               "shs" or "u3p" or "vb" or "vbe" or
               "vbs" or
               "vbscript" or "ws" or "wsf";
    }

    public string GetExtensionWithoutDot(string extension)
    {
        return extension?.Length > 0 ? extension[1..] : "";
    }
#if underdevelop
//Method to put file into database from drive:
/*public void databaseFilePut(string varFilePath)
	{
		byte[] file;
		using (var stream = new FileStream(varFilePath, FileMode.Open, FileAccess.Read))
		{
			using (var reader = new BinaryReader(stream))
			{
				file = reader.ReadBytes((int)stream.Length);
			}
		}
		using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
		using (var sqlWrite = new SqlCommand("INSERT INTO Raporty (RaportPlik) Values(@File)", varConnection))
		{
			sqlWrite.Parameters.Add("@File", SqlDbType.VarBinary, file.Length).Value = file;
			sqlWrite.ExecutedontGiveOutput();
		}
	}
	//This method is to get file from database and save it on drive:
	public void databaseFileRead(string varID, string varPathToNewLocation)
	{
		using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
		using (var sqlQuery = new SqlCommand(@"SELECT [RaportPlik] FROM [dbo].[Raporty] WHERE [RaportID] = @varID", varConnection))
		{
			sqlQuery.Parameters.AddWithValue("@varID", varID);
			using (var sqlQueryResult = sqlQuery.ExecuteReader())
				if (sqlQueryResult != null)
				{
					sqlQueryResult.Read();
					var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
					sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
					using (var fs = new FileStream(varPathToNewLocation, FileMode.Create, FileAccess.Write))
						fs.Write(blob, 0, blob.Length);
				}
		}
	}*/
//This method is to get file from database and put it as MemoryStream:
	public MemoryStream databaseFileRead(string varID)
	{
		MemoryStream memoryStream = new MemoryStream();
		using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
		using (var sqlQuery =
 new SqlCommand(@"SELECT [RaportPlik] FROM [dbo].[Raporty] WHERE [RaportID] = @varID", varConnection))
		{
			sqlQuery.Parameters.AddWithValue("@varID", varID);
			using (var sqlQueryResult = sqlQuery.ExecuteReader())
				if (sqlQueryResult != null)
				{
					sqlQueryResult.Read();
					var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
					sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
					//using (var fs = new MemoryStream(memoryStream, FileMode.Create, FileAccess.Write)) {
					memoryStream.Write(blob, 0, blob.Length);
					//}
				}
		}
		return memoryStream;
	}
	//This method is to put MemoryStream into database:
	public int databaseFilePut(MemoryStream fileToPut)
	{
		int varID = 0;
		byte[] file = fileToPut.ToArray();
		const string preparedCommand = @"
                    INSERT INTO [dbo].[Raporty]
                               ([RaportPlik])
                         VALUES
                               (@File)
                        SELECT [RaportID] FROM [dbo].[Raporty]
            WHERE [RaportID] = SCOPE_IDENTITY()
                    ";
		using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
		using (var sqlWrite = new SqlCommand(preparedCommand, varConnection))
		{
			sqlWrite.Parameters.Add("@File", SqlDbType.VarBinary, file.Length).Value = file;

			using (var sqlWriteQuery = sqlWrite.ExecuteReader())
				while (sqlWriteQuery != null && sqlWriteQuery.Read())
				{
					varID = sqlWriteQuery["RaportID"] is int ? (int)sqlWriteQuery["RaportID"] : 0;
				}
		}
		return varID;
	}
#endif

    private string _basePath;

    private string BasePath //todo check references for specific root candidates
    {
        get
        {
            if (string.IsNullOrEmpty(_basePath))
            {
                Microsoft.Extensions.Configuration.IConfiguration config = DependencyInjectionHolder.Instance.Configuration;
                _basePath = config["FilePath"];
                if (string.IsNullOrWhiteSpace(_basePath))
                {
                    _basePath = "C:\\BpmsUploads";
                }
            }

            return _basePath;
        }
    }

    private string _baseServicesPath;

    private string BaseServicesPath //todo check references for specific root candidates
    {
        get
        {
            if (string.IsNullOrEmpty(_baseServicesPath))
            {
                Microsoft.Extensions.Configuration.IConfiguration config = DependencyInjectionHolder.Instance.Configuration;
                _baseServicesPath = config["FileServicesPath"];
                if (string.IsNullOrWhiteSpace(_baseServicesPath))
                {
                    _baseServicesPath = BasePath;
                }
            }

            return _baseServicesPath;
        }
    }

    private bool SaveFileToServer(string fileName, string fileInfoData, DateTime curTime,
        bool isPhysicalLocationDateBased, DateBasedResolution dateResolution, bool isPhysicalLocationCategoryBased,
        string categoryName)
    {
        if (string.IsNullOrEmpty(fileInfoData))
        {
            return false;
        }

        string path = GetFilePath(AcquireRoot(categoryName), curTime, isPhysicalLocationDateBased,
            dateResolution, isPhysicalLocationCategoryBased, categoryName);
        CreateDirectoryIfNotExists(path);
        string fullFileName = Path.Combine(path, fileName);
        File.WriteAllBytes(fullFileName, Convert.FromBase64String(fileInfoData));
        return true; // ConvertFileToRar(path, fullFileName);
    }

    private string AcquireSpecificRoot(string categoryName)
    {
        QueryUtility fileCategory = new QueryUtility("SystemConfigs", "FileCategory", "FileManager.5")
            .Where($"Id='{categoryName}'")
            .SelectField("SpecificRoot");
        if (!fileCategory.GetDocuments())
        {
            return null;
        }

        ElasticObject record = fileCategory.GetRecord();
        fileCategory.ReleaseQuery();
        return record?.GetString("SpecificRoot");
    }

    private string AcquireRoot(string categoryName)
    {
        string specificRoot = categoryName != null ? AcquireSpecificRoot(categoryName) : null;
        return string.IsNullOrEmpty(specificRoot) ? BasePath : specificRoot;
    }

    private bool MoveFileToServer(string fileName, string sourcePath, DateTime curTime,
            bool isPhysicalLocationDateBased, DateBasedResolution dateResolution, bool isPhysicalLocationCategoryBased,
            string categoryName, ref ExceptionInfos errors)
    {
        if (string.IsNullOrEmpty(sourcePath))
        {
            return false;
        }

        string fullFilePath = string.Empty;
        try
        {
            string path = GetFilePath(AcquireRoot(categoryName), curTime, isPhysicalLocationDateBased, dateResolution, isPhysicalLocationCategoryBased, categoryName);
            CreateDirectoryIfNotExists(path);
            fullFilePath = Path.Combine(path, fileName);
            try
            {
                File.Move(sourcePath, fullFilePath);
            }
            catch (IOException e)
            {
                if (e.GetType() == typeof(IOException))
                {
                    Logger.LogTrace("Retry move {0} {1}", sourcePath, fullFilePath);
                    File.Move(sourcePath, fullFilePath);
                }
                else
                {
                    throw;
                }
            }
            Directory.Delete(Path.GetDirectoryName(sourcePath), true);
        }
        catch (Exception e)
        {
            Logger.LogError("MoveFileToServer {0} {1}", sourcePath, fullFilePath);
            Logger.LogError(e, e.Message);
            ExceptionInfos.AppendError(ref errors, new ExceptionInfo(string.Empty, new Exception("در انتقال فایل خطایی رخ داد.")));
            return false;
        }

        return true;
    }

    private void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            bool directoryExists = Directory.Exists(path);
            if (!directoryExists)
            {
                Logger.LogTrace("{0} {1}", path, true);
                Directory.CreateDirectory(path);
            }
        }
    }
   

    #region rar convertor
    public string GetFilePath(string path, DateTime curTime, bool isPhysicalLocationDateBased,
        DateBasedResolution dateResolution, bool isPhysicalLocationCategoryBased, string categoryName)
    {
        if (isPhysicalLocationCategoryBased)
        {
            path = Path.Combine(path, categoryName.Replace(".", "\\"));
        }

        if (isPhysicalLocationDateBased)
        {
            path = dateResolution switch
            {
                DateBasedResolution.None or DateBasedResolution.Monthly => Path.Combine(path, curTime.Year.ToString(), curTime.Month.ToString()),
                DateBasedResolution.Daily => Path.Combine(path, curTime.Year.ToString(), curTime.Month.ToString(), curTime.Day.ToString()),
                _ => throw new ArgumentOutOfRangeException(nameof(dateResolution), dateResolution, null),
            };
        }

        return path;
    }

    public string GetFileFullName(string guid, DateTime time, bool isPhysicalLocationDateBased,
        DateBasedResolution dateResolution, bool isPhysicalLocationCategoryBased,
        string categoryName, string extension, string specificRoot, bool withoutRoot)
    {
        string root = withoutRoot ? "" : string.IsNullOrEmpty(specificRoot) ? BasePath : specificRoot;
        return Path.Combine(
            GetFilePath(root, time, isPhysicalLocationDateBased, dateResolution, isPhysicalLocationCategoryBased,
                categoryName),
            guid + "." + extension);
    }

    public string GetServicesFilePath(string guid, DateTime time, bool isPhysicalLocationDateBased,
        DateBasedResolution dateResolution, bool isPhysicalLocationCategoryBased,
        string categoryName, string extension, string specificRoot, bool withoutRoot)
    {
        string root = withoutRoot ? "" : string.IsNullOrEmpty(specificRoot) ? BaseServicesPath : specificRoot;
        string result =
            Path.Combine(
                GetFilePath(root, time, isPhysicalLocationDateBased, dateResolution, isPhysicalLocationCategoryBased,
                    categoryName), guid + "." + extension);
        return BaseServicesPath.StartsWith("/") ? result.Replace('\\', '/') : result;
    }

    public string GetMimeType(string extension)
    {
        string extensionWithDot = extension.StartsWith(".") ? extension : "." + extension;
        return NeoMimeTypes.GetMimeType(extensionWithDot);
    }

    public string UploadedFilesPath()
    {
        string p = DependencyInjectionHolder.Instance.Configuration["UploadChunksPath"];
        return string.IsNullOrEmpty(p) ? "App_Data" : p;
    }
    #endregion rar convertor
}
#endif