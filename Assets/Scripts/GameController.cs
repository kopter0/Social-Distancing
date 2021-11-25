using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{

    public GameObject npcPrefab;
    public int npc_count = 10;
    public Transform loseCanvas;
    public Transform winCanvas;
    public Transform player;
    public GameObject page1, page2;
    public AudioSource introSound, backgroundSound;

    public GameObject pressSpaceMessage;
    //[HideInInspector] public MyState gameState = MyState.StoryBoardPage1;
    [HideInInspector] public MyState gameState = MyState.InGame;
    public enum MyState
    {
        MainMenu,
        StoryBoardPage1, 
        StoryBoardPage2,
        InGame,
        PostGame

    }
    private Vector3 player_init;
    private Vector3 camera_init;

    private Image whiteScreen;
    private Text winText;
    private AudioSource winSound;

    private Image blackScreen;
    private Text loseText;

    private Image confimedCaseScreen;
    private Text confimedText;

    private AudioSource loseSound;


    private float loseSoundDuration;
    private NPCInfo[] npcs;
    private int width = 10;

    System.Random rand;

    private struct NPCInfo
    {   
        public GameObject npc;
        /*public float min_x, max_x;*/
        public int direction;
        public float speed;
    };

    // Start is called before the first frame update
    void Start()
    {
        rand = new System.Random();

        player_init = player.position;
        camera_init = Camera.main.transform.position;

        // Set up Lose Screen
        blackScreen = loseCanvas.GetComponentsInChildren<Image>()[0];
        confimedCaseScreen = loseCanvas.GetComponentsInChildren<Image>()[1];

        loseText = loseCanvas.GetComponentsInChildren<Text>()[0];
        confimedText = loseCanvas.GetComponentsInChildren<Text>()[1];

        loseSound = loseCanvas.GetComponentInChildren<AudioSource>();
        loseSoundDuration = loseSound.clip.length;

        whiteScreen = winCanvas.GetComponentInChildren<Image>();
        winText = winCanvas.GetComponentInChildren<Text>();
        winSound = winCanvas.GetComponentInChildren<AudioSource>();
        //GenerateNPCs();
        gameState = MyState.StoryBoardPage1;
        introSound.Play();
        
    }

    // Update is called once per frame
    void Update()
    {    
        if (gameState.Equals(MyState.InGame))
        {
            for (int i = 0; i < npc_count; i++)
            {
                NPCInfo cur = npcs[i];
                Transform curTrans = cur.npc.transform;
                curTrans.position += cur.direction * cur.speed * Vector3.right * Time.deltaTime * 100.0f;
                if (curTrans.position.x < -width * 2.5f)
                {
                    npcs[i].direction = 1;
                }
                if (curTrans.position.x > width * 2.5f)
                {
                    npcs[i].direction = -1;
                }
            }
        }
    }

    public void SwitchGameState()
    {
        switch (gameState)
        {
            case MyState.PostGame:
                RestartGame(); 
                pressSpaceMessage.SetActive(false);
                break;
            case MyState.StoryBoardPage1:
                page1.SetActive(false);
                page2.SetActive(true);
                gameState = MyState.StoryBoardPage2;
                break;
            case MyState.StoryBoardPage2:
                page2.SetActive(false);
                pressSpaceMessage.SetActive(false);
                introSound.Stop();
                RestartGame();
                break;
            default:
                break;
        }
    }

    private void GenerateNPCs()
    {
        npcs = new NPCInfo[npc_count];
        
        int z = -35;
        for (int i = 0; i < npc_count; i++)
        {
            GameObject temp = Instantiate(npcPrefab);
            float x = rand.Next(-50 * width, 50 * width) / 10.0f;
            float y = rand.Next(10, 20) / 10.0f;
            temp.transform.position = new Vector3(x, y, z);
            temp.transform.Find("Sphere").position -= Vector3.up * temp.transform.Find("Sphere").position.y;
            z += 7;
            NPCInfo tempInfo = new NPCInfo();
            tempInfo.npc = temp;
            tempInfo.speed = rand.Next(10, 25) / 100.0f;
            tempInfo.direction = rand.Next(0, 1) * -2 + 1;
            npcs[i] = tempInfo;
        }
    }

    public void RestartGame()
    {

        StopAllCoroutines();
        loseSound.Stop();
        winSound.Stop();
        backgroundSound.Play();
        player.position = player_init;
        Camera.main.transform.position = camera_init;
        blackScreen.color = loseText.color = new Color(0, 0, 0, 0);
        whiteScreen.color = winText.color = new Color(0, 0, 0, 0);

        if (npcs != null)
        {
            foreach (NPCInfo npcInfo in npcs){
                Destroy(npcInfo.npc);
            }
        }

        GenerateNPCs();
        gameState = MyState.InGame;
    }

    IEnumerator Win()
    {
        backgroundSound.Stop();
        winSound.Play();

        float a1 = 0, a2 = 0;

        while (a1 < 1)
        {
            a1 += Time.deltaTime;
            whiteScreen.color = new Color(1, 1, 1, a1);
            yield return null;
        }

        while (a2 < 1)
        {
            a2 += Time.deltaTime;
            winText.color = new Color(0, 0, 0, a2);
            yield return null;
        }

        pressSpaceMessage.SetActive(true);

        yield return null;
    }

    IEnumerator Lose()
    {
        backgroundSound.Stop();
        loseSound.Play();

        confimedText.text = string.Format("Daejeon Case No {1} Confimed{0}{0}Travel History: *Near you *{0}{0}Anyone who got into contact(you) have to be quarantined", System.Environment.NewLine, rand.Next(200, 300));
        
        float a1 = 0, a2 = 0, a3 = 0;


        while(a1 < 1)
        {
            a1 += Time.deltaTime;
            confimedCaseScreen.color = new Color(1, 1, 1, a1);
            confimedText.color = new Color(0, 0, 0, a1);
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        

        confimedText.color = confimedCaseScreen.color = new Color(0, 0, 0, 0);

        while (a2 < 1)
        {
            a2 += Time.deltaTime;
            blackScreen.color = new Color(0, 0, 0, a2);
            yield return null;
        }
        
        yield return null;

        while (a3 < 1)
        {
            a3 += Time.deltaTime;
            loseText.color = new Color(1, 0, 0, a3);
            yield return null;
        }

        pressSpaceMessage.SetActive(true);


        yield return null;
    }
    
}
