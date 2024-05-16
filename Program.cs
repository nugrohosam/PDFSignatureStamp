// See https://aka.ms/new-console-template for more information
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;

Console.WriteLine("Hello, World!");

var signatureNames = new List<String> {
    "Doni Alamsyah",
    "Sanusi Harahab",
    "Rozak Danuarta",
};

var surename = signatureNames.Select(
    x => string.Concat(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
         .Where(x => x.Length >= 1 && char.IsLetter(x[0]))
         .Select(x => char.ToUpper(x[0])))
).ToArray();


// create dotnet load pdf file
// Replace with your PDF path
var oldFile = Path.GetFullPath("./test.pdf");
var newFile = Path.GetFullPath("./stempped.pdf");
// Replace with your desired text
var stampText = String.Join(",", surename);
// Replace with starting and ending page numbers (1-based)

try{
    System.IO.File.Delete(newFile);
} finally {
    Console.WriteLine("Deleted");
}

PdfReader reader = new(oldFile);
Rectangle size = reader.GetPageSizeWithRotation(1);

var tempFiles = new List<String>();
for (var i = 1; i <= reader.NumberOfPages; i++) {
    Document document = new(size);
    var tempFile = Path.GetFullPath("./tempFIles/temp-"+ + i + ".pdf");
    tempFiles.Add(tempFile);
    FileStream fs = new(tempFile, FileMode.Create, FileAccess.Write);
    PdfWriter writer = PdfWriter.GetInstance(document, fs);
    document.Open();

    PdfContentByte cb = writer.DirectContent;
    PdfImportedPage page = writer.GetImportedPage(reader, i);
    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
    cb.SetColorFill(BaseColor.BLACK);
    cb.SetFontAndSize(bf, 12);

    cb.BeginText();
    cb.ShowTextAligned(Element.ALIGN_LEFT, stampText, 20, 5, 0);
    cb.EndText();

    cb.AddTemplate(page, 10, 20);
    document.Close();
    writer.Close();
    fs.Close();
}

reader.Close();

PdfImportedPage importedPage;

Document sourceDocument = new();
PdfCopy pdfCopyProvider = new(sourceDocument, new FileStream(newFile, FileMode.Create));

//output file Open  
sourceDocument.Open();

//files list wise Loop  
for (int f = 0; f < tempFiles.Count(); f++)
{
    PdfReader reader2 = new(tempFiles[f]);

    importedPage = pdfCopyProvider.GetImportedPage(reader2, 1);
    pdfCopyProvider.AddPage(importedPage);

    reader2.Close();
}

//save the output file  
sourceDocument.Close();