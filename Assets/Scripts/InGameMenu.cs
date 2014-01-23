using UnityEngine;
using System.Collections;

public class InGameMenu : MonoBehaviour {
    GameManager mgr;

	void OnEnable() {
        mgr = GetComponent<GameManager>();

        Debug.Log("Do menu setup stuff");

        mgr.gameFSM.ChangeState(GameManager.GameState.InGameMenu);
	}
	
	void OnDisable() {
        Debug.Log("Do menu teardown stuff");

        mgr.gameFSM.ChangeState(mgr.gameFSM.PrevState);
	}
}
