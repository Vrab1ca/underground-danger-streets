using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public Transform player;
    public string xKey = "player_x";
    public string yKey = "player_y";
    public string zKey = "player_z";

    public void Save()
    {
        if (player == null) return;

        PlayerPrefs.SetFloat(xKey, player.position.x);
        PlayerPrefs.SetFloat(yKey, player.position.y);
        PlayerPrefs.SetFloat(zKey, player.position.z);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (player == null) return;

        if (!PlayerPrefs.HasKey(xKey)) return;

        Vector3 pos = new Vector3(
            PlayerPrefs.GetFloat(xKey),
            PlayerPrefs.GetFloat(yKey),
            PlayerPrefs.GetFloat(zKey)
        );

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        player.position = pos;

        if (controller != null) controller.enabled = true;
    }
}
