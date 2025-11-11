using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PathLengthTest
{
    private static string logFilePath = Path.Combine(Application.persistentDataPath, "PathLengthLog.txt");

    public static void TestPathLength(List<Vertex> path)
    {
        Debug.Log("called");
        float length = 0;
        if (path == null)
        {
            Debug.LogError("RVG Test PathLengthTest: path is null!");
            return;
        }

        for (int i = 0; i < path.Count - 1; i++)
        {
            length += path[i].Distance(path[i + 1].getPosXZ());
        }

        Debug.Log($"RVG Test {GameManager.Instance.GetTypeRVG()} ---- Path length: {length:F3}");
        // try
        // {
        //     string logLine = $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss} - Path length: {length:F3} - Vertices: {path.Count}";
        //     File.AppendAllText(logFilePath, logLine + "\n");
        // }
        // catch (IOException e)
        // {
        //     Debug.LogError($"Failed to write path length log: {e.Message}");
        // }
    }
    public static void ShowLogLocation()
    {
        Debug.LogWarning($"[PathLengthTest] Log file saved at: {logFilePath}");
    }
}
