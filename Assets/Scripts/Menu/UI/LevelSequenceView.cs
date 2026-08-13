using System.Collections.Generic;
using Menu.Levels;
using UnityEngine;
using VContainer;

namespace Menu.UI
{
    public class LevelSequenceView : MonoBehaviour
    {
        [SerializeField] private List<StartLevelButton> levelButtons = new();
        
        private SetupLevelSequence _setupLevelSequence;
        
        public void SetupButtonsView(int value, bool isOneLevel)
        {
            Debug.Log("WIDLE: "+ (levelButtons.Count / 2 - 1));
            if (isOneLevel)
            {
                for (int i = 0; i < levelButtons.Count; i++)
                {
                    if (i == 2)
                    {
                        levelButtons[i].SetNumber(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[0].LevelNumber);
                        levelButtons[i].SetLabel("∞");
                        levelButtons[i].gameObject.SetActive(true);
                        continue;
                    }
                    levelButtons[i].gameObject.SetActive(false);
                }
                return;
            }
            for (int i = 0; i < levelButtons.Count; i++)
            {
                levelButtons[i].gameObject.SetActive(true);
                levelButtons[i].SetNumber(_setupLevelSequence.CurrentLevelSequence.LevelConfigs[i].LevelNumber);
                levelButtons[i].SetLabel();
                if (levelButtons[i].Number > value)
                    levelButtons[i].SetButtonInteractable(false);
            }
        }
        
        private void OnValidate()
        {
            if (levelButtons.Count != 5)
                Debug.LogWarning("amount buttons must be 5");
        }
        
        [Inject] private void Construct(SetupLevelSequence setupLevelSequence)
        {
            _setupLevelSequence = setupLevelSequence;
        }
    }
}