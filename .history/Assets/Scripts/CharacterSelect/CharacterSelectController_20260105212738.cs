using UnityEngine;
using TMPro;
using System.IO;

public class CharacterSelectController : MonoBehaviour
{
    [Header("Input")]
    public TMP_InputField playerNameInput;
    public TMP_InputField mapNameInput;

    [Header("Models")]
    public GameObject barbarianModel;
    public GameObject mageModel;

    private Animator barbarianAnimator;
    private Animator mageAnimator;

    private CharacterType selectedCharacter = CharacterType.None;

    private void Start()
    {
        barbarianAnimator = barbarianModel.GetComponent<Animator>();
        mageAnimator = mageModel.GetComponent<Animator>();
    }

    // ================= CHARACTER SELECT =================

    public void SelectBarbarian()
    {
        Debug.Log("CLICK BARBARIAN"); // 👈 PHẢI HIỆN
        selectedCharacter = CharacterType.Barbarian;

        barbarianAnimator.SetTrigger("Cheer");
    }

    public void SelectMage()
    {
        selectedCharacter = CharacterType.Mage;

        mageAnimator.SetTrigger("Cheer");

    }


    // ================= START GAME =================

    public void StartGame()
    {
        if (selectedCharacter == CharacterType.None)
        {
            Debug.LogWarning("Chưa chọn nhân vật!");
            return;
        }

        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            Debug.LogWarning("Chưa nhập tên người chơi!");
            return;
        }

        GameSetupData data = new GameSetupData
        {
            playerName = playerNameInput.text,
            mapName = mapNameInput.text,
            characterType = selectedCharacter
        };

        SaveToJson(data);
    }

    private void SaveToJson(GameSetupData data)
    {
        string folder = GamePaths.GameDataFolder;

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GamePaths.CharacterDataPath, json);

        Debug.Log("Saved character data to: " + GamePaths.CharacterDataPath);
    }

}
