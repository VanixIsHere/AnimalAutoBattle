using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class MatchmakerClient : MonoBehaviour
{
    [SerializeField] private string serverUrl = "https://localhost:7084/matchmaking/join";
    [SerializeField] private string playerId;
    [SerializeField] private TMP_Text responseText;

    void Awake()
    {
        playerId = "Player_" + Random.Range(1000, 9999);
    }

    public void JoinMatchmaking()
    {
        StartCoroutine(SendJoinRequest());
    }

    private IEnumerator SendJoinRequest()
    {
        var payload = new JoinPayload { playerId = playerId, MMR = 1200 };
        string json = JsonUtility.ToJson(payload);

        using var request = new UnityWebRequest(serverUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

#if UNITY_EDITOR || UNITY_STANDALONE
        request.certificateHandler = new BypassCertificate(); // Local dev SSL bypass
#endif

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Matchmaking failed: " + request.error);
        }
        else
        {
            responseText.text = request.downloadHandler.text;
            Debug.Log("Matchmaking response: " + request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class JoinPayload
    {
        public string playerId;
        public int MMR;
    }

    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificate) => true;
    }
}
