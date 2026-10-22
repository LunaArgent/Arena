using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gracz : MonoBehaviour
{
    public int hp = 100;
    public int maxHp = 200;
    public Slider pasekHp;
    public Slider pasekXP;
    public string nazwa = "Bohater";
    public int poziom = 1;
    public int doswiadczenie = 0;
    public int doswiadczenieNaPoziom = 50;
    public TMPro.TMP_Text tekstHp;
    public float predkosc = 5f;
    public float zasiegGracza = 2f;
    public TMPro.TMP_Text tekstPorazki;
    public TMPro.TMP_Text tekstPoziomu;
    public UnityEngine.GameObject przyciskPonownie;
    private bool atakuje = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(nazwa + " |Poziom: " + poziom);
        hp = 100;
        if (PlayerPrefs.GetInt("TrybKontynuacji", 0) == 1)
        {
            poziom = PlayerPrefs.GetInt("Poziom", 1);
            doswiadczenie = PlayerPrefs.GetInt("XP", 0);
            doswiadczenieNaPoziom = PlayerPrefs.GetInt("XPNaPoziom", 50);
            PlayerPrefs.SetInt("TrybKontynuacji", 0);
        }
        else
        {
            poziom = 1;
            doswiadczenie = 0;
            doswiadczenieNaPoziom = 50;
        }
        OtrzymajObrazenia(20);
        Lecz(30);
        pasekHp.value = hp;
        tekstHp.text = "HP:" + hp + "/" + maxHp;
        if (tekstPoziomu != null)
        {
            tekstPoziomu.gameObject.SetActive(false);
            tekstPoziomu.text = "Poziom: " + poziom + " | XP: " + doswiadczenie + "/" + doswiadczenieNaPoziom;
        }
        if (pasekXP != null)
        {
            pasekXP.minValue = 0;
            pasekXP.maxValue = doswiadczenieNaPoziom;
            pasekXP.value = doswiadczenie;
        }
    }
    public void OtrzymajObrazenia(int obrazenia)
    {
        if (hp == 0)
        {
            return;
        }
        hp -= obrazenia;
        pasekHp.value = hp;
        tekstHp.text = "HP:" + hp + "/" + maxHp;
        if (hp < 0)
        {
            hp = 0;
        }
        if (hp == 0)
        {
            Debug.Log("Gracz nie żyje");
            GameObject tekstZwycienstwa = GameObject.Find("TekstZwyciestwa");
            if (tekstZwycienstwa != null)
            {
                tekstZwycienstwa.SetActive(true);
            }
            if (tekstPorazki != null)
            {
                tekstPorazki.gameObject.SetActive(true);
            }
            if (przyciskPonownie != null)
            {
                przyciskPonownie.SetActive(true);
            }

            return;
        }
    } 
    void Lecz(int leczenie)
        {
        if (hp == 0)
        {
            return;
        }
            hp += leczenie;
            if (hp > maxHp)
            {
                hp = maxHp;
            }
        pasekHp.value = hp;
        tekstHp.text = "HP:" + hp + "/" + maxHp;
        }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0 || GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            return;
        }
        float x = UnityEngine.InputSystem.Keyboard.current.dKey.isPressed ? 1 : 0;
        x -= UnityEngine.InputSystem.Keyboard.current.aKey.isPressed ? 1 : 0;
        float z = UnityEngine.InputSystem.Keyboard.current.wKey.isPressed ? 1 : 0;
        z -= UnityEngine.InputSystem.Keyboard.current.sKey.isPressed ? 1 : 0;
        transform.position += new Vector3(x, 0, z) * predkosc * Time.deltaTime;
        if (UnityEngine.InputSystem.Keyboard.current.hKey.wasPressedThisFrame)
        {
            Lecz(10 + (poziom - 1) *10);
            Debug.Log("Aktualne HP:" + hp);
        }
        if (UnityEngine.InputSystem.Keyboard.current.jKey.wasPressedThisFrame)
        {
            if (hp <= 0)
            {
                return;
            }
            GameObject[] przeciwnicy = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject najblizszy = null;
            float najmniejszaOdleglosc = zasiegGracza;
            foreach (GameObject przeciwnik in przeciwnicy)
            {
                float odleglosc = Vector3.Distance(transform.position, przeciwnik.transform.position);
                if (odleglosc <= najmniejszaOdleglosc)
                {
                    najmniejszaOdleglosc = odleglosc;
                    najblizszy = przeciwnik;
                }
            }
            if (najblizszy != null)
            {
                Przeciwnik skryptPrzeciwnika = najblizszy.GetComponent<Przeciwnik>();
                if (skryptPrzeciwnika != null)
                {
                    StartCoroutine(AnimacjaAtaku(najblizszy.transform));
                    skryptPrzeciwnika.OtrzymajObrazenia(10 + (poziom - 1)*5);
                }
            }
        }

    }
   private System.Collections.IEnumerator
        AnimacjaAtaku(UnityEngine.Transform cel)
    {
        Vector3 pozycjaStartowa = transform.position;
        Vector3 kierunek = cel.position - transform.position;
        kierunek.y = 0;
        if (kierunek != Vector3.zero)
        {
            kierunek.Normalize();
        }
        Vector3 pozycjaAtaku = pozycjaStartowa + kierunek * 0.35f;
        transform.position = pozycjaAtaku;
        yield return new WaitForSeconds(0.1f);
        transform.position = pozycjaStartowa;
    }
    public void ZagrajPonownie()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Kontynuuj()
    {
        PlayerPrefs.SetInt("TrybKontynuacji", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void DodajDoswiadczenie(int ile)
    {
        tekstPoziomu.gameObject.SetActive(true);
        CancelInvoke("UkryjTekstPoziomu");
        Invoke("UkryjTekstPoziomu", 3f);
        doswiadczenie += ile;
        if (pasekXP != null)
        {
            pasekXP.maxValue = doswiadczenieNaPoziom;
            pasekXP.value = doswiadczenie;
        }
        if (tekstPoziomu != null)
        {
            tekstPoziomu.text = "Poziom: " + poziom + " | XP: " + doswiadczenie + "/" + doswiadczenieNaPoziom;
        }
        if (doswiadczenie >= doswiadczenieNaPoziom)
        {
            doswiadczenie -= doswiadczenieNaPoziom;
            poziom++;
            if (tekstPoziomu != null)
            {
                tekstPoziomu.text = "Poziom: " + poziom + " | XP: " + doswiadczenie + "/" + doswiadczenieNaPoziom;
            }
            doswiadczenieNaPoziom += 25;
            if (pasekXP != null)
            {
                pasekXP.maxValue = doswiadczenieNaPoziom;
                pasekXP.value = doswiadczenie;
            }
            Debug.Log("Awans ! Nowy poziom" + poziom);
        }
        Debug.Log("Doswiadczenie: " + doswiadczenie);
        PlayerPrefs.SetInt("Poziom", poziom);
        PlayerPrefs.SetInt("XP", doswiadczenie);
        PlayerPrefs.SetInt("XPNaPoziom", doswiadczenieNaPoziom);
        PlayerPrefs.Save();
    }
    void UkryjTekstPoziomu()
    {
        if(tekstPoziomu != null)
        {
            tekstPoziomu.gameObject.SetActive(false);
        }
    }
}