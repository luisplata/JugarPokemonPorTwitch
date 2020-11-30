using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityGB;
using TwitchBot;
using CielaSpike;
using System;

public class DefaultEmulatorManager : UnityBot
{
	public string Filename;
	public Renderer ScreenRenderer;

	public EmulatorBase Emulator
	{
		get;
		private set;
	}

	private Dictionary<KeyCode, EmulatorBase.Button> _keyMapping;
    [SerializeField] private int maxRetry;
    [SerializeField] private TextMeshProUGUI chat;

    // Use this for initialization
    void Start()
	{
        chat.text = "";
		// Init Keyboard mapping
		_keyMapping = new Dictionary<KeyCode, EmulatorBase.Button>();
		_keyMapping.Add(KeyCode.UpArrow, EmulatorBase.Button.Up);
		_keyMapping.Add(KeyCode.DownArrow, EmulatorBase.Button.Down);
		_keyMapping.Add(KeyCode.LeftArrow, EmulatorBase.Button.Left);
		_keyMapping.Add(KeyCode.RightArrow, EmulatorBase.Button.Right);
		_keyMapping.Add(KeyCode.Z, EmulatorBase.Button.A);
		_keyMapping.Add(KeyCode.X, EmulatorBase.Button.B);
		_keyMapping.Add(KeyCode.Space, EmulatorBase.Button.Start);
		_keyMapping.Add(KeyCode.LeftShift, EmulatorBase.Button.Select);


		// Load emulator
		IVideoOutput drawable = new DefaultVideoOutput();
		IAudioOutput audio = GetComponent<DefaultAudioOutput>();
		ISaveMemory saveMemory = new DefaultSaveMemory();
		Emulator = new Emulator(drawable, audio, saveMemory);
		ScreenRenderer.material.mainTexture = ((DefaultVideoOutput) Emulator.Video).Texture;

		gameObject.GetComponent<AudioSource>().enabled = false;
		StartCoroutine(LoadRom(Filename));

        //twitch
        commands = new Dictionary<string, BotCommand>();
        commands.Add("!a", new BotCommand((a, b) => { //accion de comando
            Emulator.SetInput(EmulatorBase.Button.A, true);
            this.StartCoroutineAsync(EnElProximoFrameQuitarlo(EmulatorBase.Button.A));
            AgregandoComandoAlChat(b,"A");
        }));
        commands.Add("!b", new BotCommand((a, b) => { //aqui accion de comando
            Emulator.SetInput(EmulatorBase.Button.B, true);
            this.StartCoroutineAsync(EnElProximoFrameQuitarlo(EmulatorBase.Button.B));
            AgregandoComandoAlChat(b, "B");
        }));
        commands.Add("!down", new BotCommand((a, b) => { //aqui accion de comando
            Emulator.SetInput(EmulatorBase.Button.Down, true);
            this.StartCoroutineAsync(EnElProximoFrameQuitarlo(EmulatorBase.Button.Down));
            AgregandoComandoAlChat(b, "down");
        }));
        commands.Add("!left", new BotCommand((a, b) => { //aqui accion de comando
            Emulator.SetInput(EmulatorBase.Button.Left, true);
            this.StartCoroutineAsync(EnElProximoFrameQuitarlo(EmulatorBase.Button.Left));
            AgregandoComandoAlChat(b, "left");
        }));
        commands.Add("!right", new BotCommand((a, b) => { //aqui accion de comando
            Emulator.SetInput(EmulatorBase.Button.Right, true);
            this.StartCoroutineAsync(EnElProximoFrameQuitarlo(EmulatorBase.Button.Right));
            AgregandoComandoAlChat(b, "right");
        }));
        commands.Add("!select", new BotCommand((a, b) => { //aqui accion de comando
            Emulator.SetInput(EmulatorBase.Button.Select, true);
            this.StartCoroutineAsync(EnElProximoFrameQuitarlo(EmulatorBase.Button.Select));
            AgregandoComandoAlChat(b, "select");
        }));
        commands.Add("!start", new BotCommand((a, b) => { //aqui accion de comando
            Emulator.SetInput(EmulatorBase.Button.Start, true);
            this.StartCoroutineAsync(EnElProximoFrameQuitarlo(EmulatorBase.Button.Start));
            AgregandoComandoAlChat(b, "start");
        }));
        commands.Add("!up", new BotCommand((a, b) => { //aqui accion de comando
            Emulator.SetInput(EmulatorBase.Button.Up, true);
            this.StartCoroutineAsync(EnElProximoFrameQuitarlo(EmulatorBase.Button.Up));
            AgregandoComandoAlChat(b, "up");
        }));
        

        whenNewMessage += (username, message) => Debug.Log($"{username}: {message}");
        whenNewSystemMessage += (message) => Debug.Log($"System: {message}");
        whenDisconnect += () => Debug.Log("Desconexion");
        whenStart += () => Debug.Log("Conexion");
        whenNewChater += (username) => SendMessageToChat($"{username}, bienvenido al stream!");

        this.StartCoroutineAsync(StartConnection(maxRetry));
    }

    
    IEnumerator EnElProximoFrameQuitarlo(EmulatorBase.Button boton)
    {
        yield return null;
        Emulator.SetInput(boton, false);
    }

    public void AgregandoComandoAlChat(Dictionary<string,string> quien, string comando)
    {
        quien.TryGetValue("display-name", out string value);
        chat.text += comando.ToUpper() + " -> " + value + "\n";
    }

    void Update()
	{
		// Input
		/*foreach (KeyValuePair<KeyCode, EmulatorBase.Button> entry in _keyMapping)
		{
			if (Input.GetKeyDown(entry.Key))
				Emulator.SetInput(entry.Value, true);
			else if (Input.GetKeyUp(entry.Key))
				Emulator.SetInput(entry.Value, false);
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			byte[] screenshot = ((DefaultVideoOutput) Emulator.Video).Texture.EncodeToPNG();
			File.WriteAllBytes("./screenshot.png", screenshot);
			Debug.Log("Screenshot saved.");
		}*/
	}
    


	private IEnumerator LoadRom(string filename)
	{
		string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
		Debug.Log("Loading ROM from " + path + ".");

		if (!File.Exists (path)) {
			Debug.LogError("File couldn't be found.");
			yield break;
		}

		WWW www = new WWW("file://" + path);
		yield return www;

		if (string.IsNullOrEmpty(www.error))
		{
			Emulator.LoadRom(www.bytes);
			StartCoroutine(Run());
		} else
			Debug.LogError("Error during loading the ROM.\n" + www.error);
	}

	private IEnumerator Run()
	{
		gameObject.GetComponent<AudioSource>().enabled = true;
		while (true)
		{
			// Run
			Emulator.RunNextStep();

			yield return null;
		}
	}
}
