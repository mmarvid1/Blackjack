using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    public int[] values = new int[52];
    int cardIndex = 0;

    //Nuestras variables
    int playerPoints;
    int dealerPoints;

    public Text textPlayerPoints;
    public Text textDealerPoints;

    CardHand playerHand;
    CardHand dealerHand;

    int valorRandom;
    

    bool primerTurno = true;


    private void Awake()
    {
        InitCardValues();

    }

    private void Start()
    {
        playerHand = player.GetComponent<CardHand>();
        dealerHand = dealer.GetComponent<CardHand>();

        ShuffleCards();
        StartGame();
        
    }


    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */

        for(int i=0; i<52; i++)
        {
            if (i % 13 < 10)
                values[i] = (i % 13) + 1;
            else
                values[i] = 10;
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
        for (int i = 0; i < faces.Length; i++)
        {
            valorRandom = Random.Range(i, faces.Length);

            //Baraja las imágenes
            Sprite auxFace = faces[i];
            faces[i] = faces[valorRandom];
            faces[valorRandom] = auxFace;

            //Baraja los valores
            int auxValue = values[i];
            values[i] = values[valorRandom];
            values[valorRandom] = auxValue;

        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();

            playerPoints = playerHand.points;
            dealerPoints = dealerHand.points;

        }
        /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        textPlayerPoints.text = "Puntos Jugador: " + playerPoints;
        textDealerPoints.text = "Puntos Dealer: ";


        if (dealerPoints.Equals(21) && playerPoints.Equals(21))
        {
            finalMessage.text = "¡Empate chaval!";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        else if (dealerPoints.Equals(21))
        {

            finalMessage.text = "¡Has perdido chaval!";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        else if (playerPoints.Equals(21))
        {
            finalMessage.text = "¡Has ganado chaval!";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }

        
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:*/
        int cartasRestantes = faces.Length - cardIndex;
        if (cartasRestantes <= 0) return;

        int casosFavorablesDealer = 0;
        int casosFavorables17_21 = 0;
        int casosFavorables21mas = 0;

        dealerPoints = dealerHand.points;
        playerPoints = playerHand.points;

        for (int i=cardIndex; i<52; i++)
        {
           
            //Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
            if (dealerPoints + values[i] > playerPoints && dealerPoints + values[i] <= 21)
                casosFavorablesDealer++;
            //Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
            if (playerPoints + values[i] >= 17 && playerPoints + values[i] <= 21)
                casosFavorables17_21++;
            //Probabilidad de que el jugador obtenga más de 21 si pide una carta
            if (playerPoints + values[i] > 21)
                casosFavorables21mas++;
        }

        float probDealer = (float)casosFavorablesDealer / cartasRestantes * 100;
        float prob17_21 = (float)casosFavorables17_21 / cartasRestantes * 100 ;
        float prob21mas = (float)casosFavorables21mas / cartasRestantes * 100;

        probMessage.text = "Probabilidades:\nDealer gana: " + probDealer.ToString("F1") + "%\n" +
            "Jugador obtiene entre 17 y 21: " + prob17_21.ToString("F1") + "%\n" +
            "Jugador obtiene más de 21: " + prob21mas.ToString("F1") + "%";

    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealerHand.Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        playerHand.Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        /*if(primerTurno)
        {
            //dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            dealer.GetComponent<CardHand>().InitialToggle();
            primerTurno = false;
        }*/

        //Repartimos carta al jugador
        PushPlayer();

        playerPoints = playerHand.points;

        textPlayerPoints.text = "Puntos Jugador: " + playerPoints;
        
        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (playerPoints > 21)
        {
            finalMessage.text = "¡Has perdido chaval!";
            dealer.GetComponent<CardHand>().InitialToggle();
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        else if(playerPoints.Equals(21))
        {
            finalMessage.text = "¡Has ganado chaval!";
            dealer.GetComponent<CardHand>().InitialToggle();
            hitButton.interactable = false;
            stickButton.interactable = false;
        }

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if(primerTurno)
        {
            //dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            dealer.GetComponent<CardHand>().InitialToggle();
            primerTurno = false;
        }

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        dealerPoints = dealerHand.points;
        playerPoints = playerHand.points;



        while (dealerPoints <= 16)
        {
            PushDealer();
            dealerPoints = dealerHand.points;
        }

        //Cuando el dealer se planta, comprobamos quién ha ganado
        if (dealerPoints > 21) finalMessage.text = "¡Has ganado chaval!";
        else if (dealerPoints > playerPoints) finalMessage.text = "¡Has perdido chaval!";
        else if (dealerPoints < playerPoints) finalMessage.text = "¡Has ganado chaval!";
        else finalMessage.text = "¡Empate chaval!";

        hitButton.interactable = false;
        stickButton.interactable = false;
        textDealerPoints.text = "Puntos Dealer: " + dealerPoints;
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        textDealerPoints.text = "";
        textPlayerPoints.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        primerTurno = true;
        ShuffleCards();
        StartGame();
    }

}