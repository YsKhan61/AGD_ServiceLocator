# Angry Monkey
  A mini-game consisting of different monkeys who hate balloons.

# Design Patterns used
  * Flyweight Design Pattern: Properties and Datas related to Monkeys, Balloons, Waves, Maps etc. are assigned in Scriptable Object assets, in order to share them among all similar objects for memory efficiency.
  * Service Locator: ​Modules like PlayerService, AudioService, WaveService, EventService need to communicate with each other. Hence, there is a GameService, that provides the references of these services who need them. GameService is a MonoBehaviour Singleton. 
  ​* Model-View-Controller: UI elements are configured with a view script that extends MonoBehaviour, responsible for only the display of data, while separate controller scripts (plain C# scripts) are created to do calculations and processes, and model scripts are created to store data and calculated values.
  * Observer Pattern: Certain game events such as ballon hit, health decrease, score, etc need to be broadcasted and listened to by UIs and other parts, without direct references between each other.

# Objective: 
  * Unlock monkeys and place them on the map to shoot the balloons of each wave.
  * Popping each balloon will give coins and balloons escaping will reduce health.


# Monkeys
  Each monkeys have different costs to unlock, costs to use, and have variety of weapons

  * Sharp Monkey

  * Ninja Monkey

  * Sniper Monkey

  * Canon Monkey

  * Super Monkey


# Balloons
  Balloons have different health, and a balloon can have multiple balloons inside that spawn when the parent balloon is popped.
  Ex: Blue balloon pops -> Red balloon spawns.

  * Red Balloon

  * Blue Balloon

  * Black Balloon

  * White Balloon

  * Orange Balloon

# Waves
  Implementation of scriptable objects to create unique waves which are easily customizable.

# Maps
  Currently, there are 4 maps, but new maps can be easily added due to modular code architecture.

# Screenshots
  ![Screenshot 2024-07-28 124738](https://github.com/user-attachments/assets/fec239c6-8df0-473d-81dc-9ab158d9aa93)
  ![Screenshot 2024-07-28 125001](https://github.com/user-attachments/assets/26c70790-bf86-48fd-895a-0ecda1d39ae2)
