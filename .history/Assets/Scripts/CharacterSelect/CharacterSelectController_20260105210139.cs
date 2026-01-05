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

        // Mặc định: tất cả Idle
        SetAllIdle();
    }

    // ================= CHARACTER SELECT =================

    public void SelectBarbarian()
    {
        selectedCharacter = CharacterType.Barbarian;

        barbarianAnimator.SetBool("isSelected", true);
        mageAnimator.SetBool("isSelected", false);
    }

    public void SelectMage()
    {
        selectedCharacter = CharacterType.Mage;

        barbarianAnimator.SetBool("isSelected", false);
        mageAnimator.SetBool("isSelected", true);
    }

    private void SetAllIdle()
    {
        barbarianAnimator.SetBool("isSelected", false);
        mageAnimator.SetBool("isSelected", false);
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
        string json = JsonUtility.ToJson(data, true);
        string path = GamePaths.CharacterDataPath;


        File.WriteAllText(path, json);

        Debug.Log("Saved data to: " + path);
        Debug.Log(json);
    }
}
