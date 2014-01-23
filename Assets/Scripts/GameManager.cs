using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour {
    public enum State { Initialize, SetupNewGame, Game, GameOver, Restart, Quit }
    public enum GameState { Idle, InGameMenu }

    public FiniteStateMachine<State> mainFSM;
    public FiniteStateMachine<GameState> gameFSM;

    public event EventHandler SettingUpNewGame;
    public event EventHandler GameStarted;
    public event EventHandler GameIsOver;
    public event EventHandler GameRestarted;
    public event EventHandler InGameMenuOpened;
    public event EventHandler InGameMenuClosed;

    InGameMenu igm;

	void Awake() {
        mainFSM = new FiniteStateMachine<State>();
        mainFSM.AddTransition(State.Initialize, State.SetupNewGame, null, InitializeNewGame, OnSettingUpNewGame);
        mainFSM.AddTransition(State.SetupNewGame, State.Game, null, () => StartCoroutine(InitializeGameLogicStuff()), null);
        mainFSM.AddTransition(State.Game, State.GameOver, OnGameIsOver);
        mainFSM.AddTransition(State.GameOver, State.Restart, null);
        mainFSM.AddTransition(State.Restart, State.SetupNewGame, null, InitializeNewGame, null);
        mainFSM.AddTransition(State.Restart, State.Quit, null);
        mainFSM.StateChanged += (object s, EventArgs e) => {
            Debug.Log("state: " + mainFSM.CurrentState.ToString() + " | game state: " + gameFSM.CurrentState.ToString());
        };

        gameFSM = new FiniteStateMachine<GameState>();
        gameFSM.AddTransition(GameState.Idle, GameState.InGameMenu, OnInGameMenuOpened);
        gameFSM.AddTransition(GameState.InGameMenu, GameState.Idle, OnInGameMenuClosed);
        gameFSM.StateChanged += (object s, EventArgs e) => {
            Debug.Log("state: " + mainFSM.CurrentState.ToString() + " | game state: " + gameFSM.CurrentState.ToString());
        };

        GameIsOver += (object s, EventArgs e) => { Debug.Log("oh no!"); };
        InGameMenuOpened += (object s, EventArgs e) => { Time.timeScale = 0f; Debug.Log("PAUSED"); };
        InGameMenuClosed += (object s, EventArgs e) => { Time.timeScale = 1f; Debug.Log("UNPAUSED"); };

        igm = GetComponent<InGameMenu>();
        mainFSM.ChangeState(State.SetupNewGame);
	}

    IEnumerator Start() {
        while(true) {
            if(mainFSM.CurrentState == State.Game) {
                if(Input.GetKeyDown(KeyCode.G)) {
                    mainFSM.ChangeState(State.GameOver);
                }
                if(Input.GetKeyDown(KeyCode.M)) {
                    igm.enabled = !igm.enabled;
                }

                InGame();
            }
            yield return null;
        }
    }

    void InGame() {        
        switch(gameFSM.CurrentState) {
            case GameState.Idle:
                break;
            case GameState.InGameMenu:
                break;
        }
    }

	void Update() {        
        if(mainFSM.CurrentState == State.Restart) {
            if(Input.GetKeyDown(KeyCode.R)) {
                mainFSM.ChangeState(State.SetupNewGame);
            }
            if(Input.GetKeyDown(KeyCode.Q)) {
                mainFSM.ChangeState(State.Quit);
            }
        }
	}

    IEnumerator InitializeGameLogicStuff() {
        yield return new WaitForSeconds(1);
        while(true) {
            yield return new WaitForSeconds(5);

            if(mainFSM.CurrentState == State.GameOver) {
                mainFSM.ChangeState(State.Restart);
                break;
            }
        }
    }

    void InitializeNewGame() {
        if(gameFSM.CurrentState != GameState.Idle)
            gameFSM.ChangeState(GameState.Idle);        
        mainFSM.ChangeState(State.Game);
    }

    void OnSettingUpNewGame(EventArgs e) {
        var settingUpNewGame = SettingUpNewGame;
        if(settingUpNewGame != null)
            settingUpNewGame(this, e);
    }

    void OnGameIsOver(EventArgs e) {
        var gameOver = GameIsOver;
        if(gameOver != null)
            gameOver(this, e);
    }

    void OnInGameMenuOpened(EventArgs e) {
        var inGameMenuOpened = InGameMenuOpened;
        if(inGameMenuOpened != null)
            inGameMenuOpened(this, e);
    }

    void OnInGameMenuClosed(EventArgs e) {
        var inGameMenuClosed = InGameMenuClosed;
        if(inGameMenuClosed != null)
            inGameMenuClosed(this, e);
    }
}
