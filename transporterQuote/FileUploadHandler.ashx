<%@ WebHandler Language="VB" Class="FileUploadHandler" %>

Imports System
Imports System.Web
Imports System.IO

Public Class FileUploadHandler : Implements IHttpHandler

    ''- FileUploadHandler.ashx
    ''   - Purpose: image upload On server.
    ''   - In:   - path 
    ''           - typeID
    ''               1 - Logo
    ''               2 - Picture & Document
    ''               4 - Upload gerber file 
    ''   - Out: image name
    Const ImageFormats = ".bmp.jpeg.jpg.png"
    Const ReportFormats = ".pdf .xls .xlxs .doc .docx"

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "text/plain"
        Try
            If context.Request.Files.Count > 0 Then

                Dim files As HttpFileCollection = context.Request.Files

                Dim typeID As Integer = 1
                Dim filePath As [String] = ""

                typeID = Val(context.Request.QueryString("typeID"))
                filePath = context.Request.QueryString("path").ToString()

                If typeID = 1 Then
                    If Directory.Exists(context.Server.MapPath("~/Logos")) = False Then
                        Directory.CreateDirectory(context.Server.MapPath("~/Logos"))
                    End If
                ElseIf typeID = 3 Then
                    If Directory.Exists(context.Server.MapPath("~/Images")) = False Then
                        Directory.CreateDirectory(context.Server.MapPath("~/Images"))
                    End If
                    If Directory.Exists(context.Server.MapPath("~/Images/Suggestions")) = False Then
                        Directory.CreateDirectory(context.Server.MapPath("~/Images/Suggestions"))
                    End If
                    filePath = "~/Images/Suggestions/"
                ElseIf typeID = 4 Then
                    If Directory.Exists(context.Server.MapPath("~/Attachment")) = False Then
                        Directory.CreateDirectory(context.Server.MapPath("~/Attachment"))
                    End If
                    filePath = "/Attachment/"

                    ' Rename the uploaded file to store timestamp and companyID
                    Dim uplodFile As String
                    'Dim taskID As Integer

                    ' Dim currentDate As DateTime = DateTime.Now.ToString()
                    Dim strExtn As String = System.IO.Path.GetExtension(files(0).FileName).ToLower
                    'taskID = Val(context.Request.QueryString("taskID"))

                    uplodFile = context.Request.PhysicalApplicationPath + filePath
                    If InStr(ReportFormats, strExtn) > 0 Then
                        'Dim filename As String = taskID.ToString + "_" + currentDate
                        'filename = filename.Replace(" ", "")
                        'filename = filename.Replace(":", "-")
                        ' filename = filename.Replace("-", "")
                        'filename = filename.Replace("/", "")
                        Dim fullPath As String

                        fullPath = uplodFile.ToString + files(0).FileName '+ strExtn.ToString
                        ' fullPath = fullPath.Replace("\", "/")
                        ' fullPath = fullPath.Replace("~", "")
                        ' fullPath = fullPath.Replace("//", "/")
                        files(0).SaveAs(fullPath)

                        Dim retFileName = filePath + files(0).FileName + strExtn.ToString
                        'retFileName = retFileName.Replace("\", "/")
                        'retFileName = retFileName.Replace("~", "")
                        'retFileName = retFileName.Replace("//", "/")

                        context.Response.Write("{""Error"":""0"",""Msg"":""File Uploaded Successfully!"",""FileName"":""" & retFileName & """}")

                        'context.Response.Write("{""Error"":""0"",""Msg"":""File Uploaded Successfully!"",""FileName"":""" & fullPath & """}")
                        Exit Sub

                    Else
                        context.Response.Write("{""Error"":""1"", ""Msg"":""Please upload proper file!""}")
                        Exit Sub
                    End If

                Else

                End If

                ' Keep renaming until another file of this name does not exist.
                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFile = files(i)
                    Dim strExtn As String = System.IO.Path.GetExtension(file.FileName).ToLower
                    Dim fname As String = ""
                    If System.IO.File.Exists(context.Server.MapPath(filePath + file.FileName)) = True Then
                        'context.Response.Write("{""Error"":""1"", ""Msg"":""File already uploaded into destination. Please choose different file!""}")
                        'Exit Sub
                        Dim fileCount As Integer = 0
                        Dim RName As [String]() = file.FileName.Split(".")
                        Do
                            fileCount &= 1
                        Loop While System.IO.File.Exists(context.Server.MapPath(System.IO.Path.Combine(filePath & RName(0) & "_" & fileCount & "." & RName(1))))

                        fname = context.Server.MapPath(System.IO.Path.Combine(filePath & RName(0) & "_" & fileCount & "." & RName(1)))

                        file.SaveAs(fname)
                        context.Response.Write("{""Error"":""0"",""Msg"":""File Uploaded Successfully!"",""FileName"":""" & RName(0) & "_" & fileCount & "." & RName(1) & """}")
                        Exit Sub
                    End If

                    If InStr(ImageFormats, strExtn) > 0 Then
                        'Dim fname As String = context.Server.MapPath(filePath + file.FileName)

                        'file.SaveAs(fname)
                        fname = context.Server.MapPath(filePath & file.FileName)

                        file.SaveAs(fname)
                        context.Response.Write("{""Error"":""0"",""Msg"":""File Uploaded Successfully!"",""FileName"":""" & file.FileName & """}")
                        Exit Sub
                    Else
                        context.Response.Write("{""Error"":""1"", ""Msg"":""Please upload only an image!""}")
                        Exit Sub
                    End If
                Next
                context.Response.Write("{""Error"":""0"", ""Msg"":""File Uploaded Successfully!""}")
            Else
                context.Response.Write("{""Error"":""1"", ""Msg"":""Please select a file to upload!""}")
                Exit Sub
            End If

        Catch ex As Exception
            context.Response.Write("{""Error"":""1"", ""Msg"":""" & ex.ToString() & """}")
        End Try
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class