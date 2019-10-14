using Dicom;
using Dicom.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
namespace fodicom
{
    public class DicomProcess
    {
        public DicomFile GetDicomFileByFilePath(string FilePath)
        {
     
            DicomFile dicom = null;
            try
            {
                dicom = DicomFile.Open(FilePath);
            }
            catch (Exception ex)
            { }
            return dicom;
        }

        public DicomFileMetaInformation GetDicomMetaInfoByDicomFile(DicomFile dicomFile)
        {
            DicomFileMetaInformation dicominfo = null;
            try
            {
                dicominfo = dicomFile.FileMetaInfo;
            }
            catch (Exception ex)
            { }
            return dicominfo;
        }

        public DicomDataset GetDicomDatasetByDicomFile(DicomFile dicomFile)
        {
            DicomDataset dicomset = null;
            try
            {
                dicomset = dicomFile.Dataset;
            }
            catch (Exception ex)
            { }
            return dicomset;
        }

        public void SaveDicomFile(DicomFile dicomFile, string FilePath)
        {
            try
            {
                dicomFile.Save(FilePath);
            }
            catch (Exception ex)
            { }
        }

        public string GetDicomTagValueByDicomDataset(DicomDataset dicomset, DicomTag dicomTag)
        {
            string TagValue = "";
            if (dicomset.TryGetString(dicomTag, out TagValue))
            { TagValue = dicomset.GetString(dicomTag); }
            return TagValue;
        }

        public string GetDicomTagValueByDicomMetaInfo(DicomFileMetaInformation dicomMetaInfo, DicomTag dicomTag)
        {
            string TagValue = "";
            if (dicomMetaInfo.TryGetString(dicomTag, out TagValue))
            { TagValue = dicomMetaInfo.GetString(dicomTag); }
            return TagValue;
        }

        public DicomDataset RemoveDicomTag(DicomDataset dicomset, DicomTag dicomTag)
        {
            return dicomset.Remove(dicomTag);
        }

        public DicomDataset AddOrUpdateDicomTag(DicomDataset dicomset, DicomTag dicomTag, string TagValue)
        {
            return dicomset.AddOrUpdate(dicomTag, TagValue);
        }
    }
}
