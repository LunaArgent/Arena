using Unity.Collections.Tests.CoreCLR.TestJobs;
using UnityEngine;

public class Przeciwnik : MonoBehaviour
{
    public float predkosc = 2f;
    public int hp = 50;
    public int maxHp = 50;
    public TMPro.TMP_Text tekstHp;
    public Transform wizualnyPasekHp;
    public Vector3 poczatkowaSkalaPaska;
    public float zasiegAtaku = 1.5f;
    public int obrazenia = 10;
    public float odstepAtakow = 1f;
    public float silaOdrzutu = 1.5f;
    public TMPro.TMP_Text tekstZwyciestwa;
    public UnityEngine.GameObject przyciskPonownie;
    public UnityEngine.GameObject przyciskKontynuuj;
    public Gracz skryptGracza;
    private float czasDoAtaku = 0f;
    private Renderer rendererPrzeciwnika;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rendererPrzeciwnika = GetComponent<Renderer>();
        poczatkowaSkalaPaska = wizualnyPasekHp.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (tekstHp != null && Camera.main != null)
        {
            tekstHp.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.8f);
        }
        GameObject gracz = GameObject.Find("Gracz");
        if (gracz != null)
        {
            czasDoAtaku -= Time.deltaTime;
            float odleglosc = Vector3.Distance(transform.position, gracz.transform.position);
            if (odleglosc <= zasiegAtaku && czasDoAtaku <= 0f)
            {
                Gracz skryptGracza = gracz.GetComponent<Gracz>();
                if (skryptGracza!= null)
                {
                    skryptGracza.OtrzymajObrazenia(obrazenia);
                    czasDoAtaku = odstepAtakow;
                }
            }
            {
                Vector3 kierunek = gracz.transform.position - transform.position;
                kierunek.y = 0;
                kierunek.Normalize();
                transform.position += kierunek * predkosc * Time.deltaTime;
            }
        }
    }
    public void OtrzymajObrazenia(int obrazenia)
    {
        StartCoroutine(MignijNaBialo());
        if (hp <= 0)
        {
            return;
        }
        hp -= obrazenia;
        if (hp < 0)
        {
            hp = 0;
        }
        if (wizualnyPasekHp != null)
        {
            float procentHp = (float)hp / maxHp;
            wizualnyPasekHp.localScale = new Vector3(poczatkowaSkalaPaska.x * procentHp, poczatkowaSkalaPaska.y, poczatkowaSkalaPaska.z);
        }
        Vector3 kierunekOdrzutu = transform.position - GameObject.Find("Gracz").transform.position;
        kierunekOdrzutu.y = 0;
        kierunekOdrzutu.Normalize();
        transform.position += kierunekOdrzutu * silaOdrzutu;
        Debug.Log("Hp przeciwnika:" + hp);
        tekstHp.text = "HP przeciwnika:" + hp + "/" + maxHp;
        if (hp == 0)
        {
            Debug.Log(" Przeciwnik pokonany!");
            GameObject gracz = GameObject.Find("Gracz");
            if (gracz != null)
            {
                Gracz skryptGracza = gracz.GetComponent<Gracz>();
                if (skryptGracza != null)
                {
                    skryptGracza.DodajDoswiadczenie(50);
                }
                if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 1)
                {
                    if (tekstZwyciestwa != null)
                    {
                        tekstZwyciestwa.gameObject.SetActive(true);
                    }
                    if (przyciskPonownie != null)
                    {
                        przyciskPonownie.SetActive(true);
                    }
                    if (przyciskKontynuuj != null)
                    {
                        przyciskKontynuuj.SetActive(true);
                    }
                }
                Destroy(gameObject);
            }
        }
    }
    private System.Collections.IEnumerator MignijNaBialo()
    {
        if (rendererPrzeciwnika == null)
            yield break;
        UnityEngine.Color poprzedniKolor = rendererPrzeciwnika.material.color;
        rendererPrzeciwnika.material.color = UnityEngine.Color.white;
        yield return new WaitForSeconds(0.1f);
        rendererPrzeciwnika.material.color = poprzedniKolor;

    }
}
