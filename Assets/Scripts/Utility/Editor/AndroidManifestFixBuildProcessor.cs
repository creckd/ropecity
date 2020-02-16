#if UNITY_ANDROID
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

class AndroidManifestFixBuildProcessor : MonoBehaviour, IPostprocessBuild
{
    public int callbackOrder { get { return 1; } }

    static string dataPath = "";
    private static Thread _OnPostprocessBuildThread;

    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        Debug.Log("OnPostprocessBuild");
        dataPath = Application.dataPath + "/../Temp";
        Application.logMessageReceived += buildLogCallback;
        if (_OnPostprocessBuildThread != null && _OnPostprocessBuildThread.IsAlive)
        {
            _OnPostprocessBuildThread.Interrupt();
            _OnPostprocessBuildThread.Abort();
            _OnPostprocessBuildThread = null;
        }
        _OnPostprocessBuildThread = new Thread(OnPostprocessBuildThread);
        _OnPostprocessBuildThread.Start();
        Debug.Log("Start OnPostprocessBuildThread");

    }

    private void OnPostprocessBuildThread()
    {
        while (true)
        {
            Debug.Log("Run AndroidManifestFixBuildProcessor");
            AndroidManifestFixBuildProcessor.AndroidManifest(dataPath);
            Thread.Sleep(200);
        }
    }

    public static void buildLogCallback(string condition, string stackTrace, LogType type)
    {
        AndroidManifestFixBuildProcessor.AndroidManifest(dataPath);
        if (condition.Contains("Build completed"))
        {
            if (_OnPostprocessBuildThread != null && _OnPostprocessBuildThread.IsAlive)
            {
                _OnPostprocessBuildThread.Interrupt();
                _OnPostprocessBuildThread.Abort();
                _OnPostprocessBuildThread = null;
            }
            Debug.Log("Stop OnPostprocessBuildThread");
            Application.logMessageReceived -= buildLogCallback;
        };

    }
    //

    public static void AndroidManifestFinished(object sender, FinishedArgs args)
    {
        if (args.LinesRemoved > 0)
        {
            Debug.Log("<color=#FF8000>File:" + args.Filename + " LinesRemoved:" + args.LinesRemoved + " TotalLines:" + args.TotalLines + " </color>");
        }
    }

    public static void AndroidManifest(string buildDir)
    {
        List<string> removedParams = new List<string>();

        removedParams.Add("android.permission.BLUETOOTH");
        removedParams.Add("android.permission.CHANGE_WIFI_MULTICAST_STATE");
        removedParams.Add("android.permission.READ_PHONE_STATE");

        TextLineRemover.OnFinished += AndroidManifestFinished;
        //   Debug.Log(buildDir);

        string[] files = Directory.GetFiles(buildDir, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string patchfile = Path.GetFullPath(file);
            string TempPath = Path.GetTempPath();
            string patchfileTMP = TempPath + Path.GetFileNameWithoutExtension(file) + ".tmp";
            TextLineRemover.RemoveTextLines(removedParams, patchfile, patchfileTMP);
        }
        TextLineRemover.OnFinished -= AndroidManifestFinished;
    }
}
/*
public class FixAndroidManifest
{
    [MenuItem("Tools/FixAndroidManifest")]
    static void FixAndroidManifestRun()
    {
        AndroidManifestFixBuildProcessor.AndroidManifest(Application.dataPath + "/../Temp");
    }
}
*/

public static class TextLineRemover
{
    public static void RemoveTextLines(IList<string> linesToRemove, string filename, string tempFilename)
    {
        // Initial values
        int lineNumber = 0;
        int linesRemoved = 0;
        System.DateTime startTime = System.DateTime.Now;

        // Read file
        using (var sr = new StreamReader(filename))
        {
            // Write new file
            using (var sw = new StreamWriter(tempFilename))
            {
                // Read lines
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    // Look for text to remove
                    if (!ContainsString(line, linesToRemove))
                    {
                        // Keep lines that does not match
                        sw.WriteLine(line);
                    }
                    else
                    {
                        // Ignore lines that DO match
                        linesRemoved++;
                        InvokeOnRemovedLine(new RemovedLineArgs { RemovedLine = line, RemovedLineNumber = lineNumber });
                    }
                }
            }
        }
        // Delete original file
        File.Delete(filename);

        // ... and put the temp file in its place.
        File.Move(tempFilename, filename);

        // Final calculations
        System.DateTime endTime = System.DateTime.Now;
        InvokeOnFinished(new FinishedArgs { LinesRemoved = linesRemoved, TotalLines = lineNumber, TotalTime = endTime.Subtract(startTime), Filename = filename });
    }

    private static bool ContainsString(string line, IEnumerable<string> linesToRemove)
    {
        foreach (var lineToRemove in linesToRemove)
        {
            if (line.Contains(lineToRemove))
                return true;
        }
        return false;
    }

    public static event RemovedLine OnRemovedLine;
    public static event Finished OnFinished;

    public static void InvokeOnFinished(FinishedArgs args)
    {
        Finished handler = OnFinished;
        if (handler != null) handler(null, args);
    }

    public static void InvokeOnRemovedLine(RemovedLineArgs args)
    {
        RemovedLine handler = OnRemovedLine;
        if (handler != null) handler(null, args);
    }
}

public delegate void Finished(object sender, FinishedArgs args);

public class FinishedArgs
{
    public int TotalLines { get; set; }
    public int LinesRemoved { get; set; }
    public System.TimeSpan TotalTime { get; set; }
    public string Filename { get; set; }
}

public delegate void RemovedLine(object sender, RemovedLineArgs args);

public class RemovedLineArgs
{
    public string RemovedLine { get; set; }
    public int RemovedLineNumber { get; set; }
}


#endif //UNITY_ANDROID