using UnityEngine;
using UnityEngine.Video;

public class IntroVideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        
        // On s'abonne à l'événement qui détecte la fin de la vidéo
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer source)
    {
        // Option 1 : Si la vidéo est dans la scène de jeu, on cache juste l'UI de la vidéo
        gameObject.SetActive(false); 

        // Option 2 : Si vous préférez charger une AUTRE scène après la vidéo, décommentez la ligne ci-dessous :
        // UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    void Update()
    {
        // Optionnel : Permettre au joueur de passer la vidéo avec la touche Entrée ou Espace
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnVideoFinished(videoPlayer);
        }
    }

    void OnDestroy()
    {
        // Toujours se désabonner des événements pour éviter les fuites de mémoire
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}