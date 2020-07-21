using UnityEngine;

namespace Level1
{
    public class Goal : MonoBehaviour
    {
        private bool onLeft;
        private GameManager manager;

        // Start is called before the first frame update
        void Start()
        {
            manager = FindObjectOfType<GameManager>();
            onLeft = transform.position.x < 0;
            GetComponent<SpriteRenderer>().color = onLeft ? Color.green : Color.red;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Ball")
            {
                // Hit by ball
                manager.scoreGoal(onLeft);
            }
        }
    }
}
