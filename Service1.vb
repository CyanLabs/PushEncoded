Imports System.Text ' Needed to encode the token to base64
Imports System.Net ' Needed for WebClient
Imports System.IO  ' Needed for FileSystem manipulation.
Imports Newtonsoft.Json.Linq ' Needed for JSON.

Public Class PushEncoded
    Dim fsw As New FileSystemWatcher() ' Created FileSystemWatcher to monitor folder (Hard Coded)
    Protected Overrides Sub OnStart(ByVal args() As String)
        AddHandler fsw.Created, New FileSystemEventHandler(AddressOf File_Created) ' Creates the handler for the when new file is created.
        fsw.Path = "D:\Media\Youtube Recordings\AutoEncode\Output\" ' Hardcoded path [TODO: add optional parameter].
        fsw.EnableRaisingEvents = True ' Enables the detection of a new file.
        fsw.Filter = "*.mp4"
    End Sub
    Public Sub File_Created(ByVal obj As Object, ByVal e As FileSystemEventArgs)
        PushMe("Encoding Finished", e.Name & " has finished being encoded by Adobe Media Encoder.") 'Sends the push notification to pushbullet API.
    End Sub
    Sub PushMe(title, message)
        Try
            Dim wc As WebClient = New WebClient ' Creates WebClient for Push
            Dim authEncoded As String = Convert.ToBase64String(Encoding.UTF8.GetBytes("reSrsH7pTwYveqlbG8StPVcOzn4pkWXO:")) ' Converts string to Base64 encoded string.
            wc.Headers(HttpRequestHeader.Authorization) = String.Format("Basic {0}", authEncoded) ' Sets HTTP Header.
            wc.Headers(HttpRequestHeader.ContentType) = "application/json" ' Sets HTTP Header.
            wc.Headers(HttpRequestHeader.Accept) = "application/json" ' Sets HTTP Header.
            Dim strJson = New JObject(New JProperty("type", "note"), New JProperty("title", title), New JProperty("body", message)) ' Creates the POST data to send to Pushbullet.
            wc.UploadString("https://api.pushbullet.com/v2/pushes", strJson.ToString) ' Uploads the POST JSON data to pushbullet.
            wc.Dispose() ' Disposes the WebClient to free resources
        Catch ex As Exception
            EventLog.WriteEntry(ex.Message, EventLogEntryType.Error)
            Exit Sub
        End Try
    End Sub
End Class
