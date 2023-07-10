# Project Overview


This document provides instructions for modifying training settings and adjusting reward/penalty values of the project.


## Training with Mlagents

To view a description of all the CLI options accepted by mlagents-learn, use the --help
```sh
mlagents-learn --help
```
The basic command for training is:
```sh
mlagents-learn <trainer-config-file> --env=<env_name> --run-id=<run-identifier>
```
1. ``<trainer-config-file>`` is the file path of the trainer configuration YAML
2. ``<env_name>``(Optional) is the name (including path) of your Unity executable containing the agents to be trained.
3. ``<run-identifier>`` is a unique name you can use to identify the results of your training runs.
4. ``initialize-from=<run-identifier> --run-id=<new-id>`` start a new training run but initialize it using an already-trained model.
5. ``--force`` to re-run a previously interrupted or completed training run and re-use the same run ID.
6. ``--resume`` to resume a previously interrupted or completed training run
Note: environment is required!

*Using an [Environment Executable](https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Learning-Environment-Executable.md)


## Modifying Training Settings

To change the training phase or type, follow these steps:
  1. Open the ``Hierarchy`` window
  2. Select the ``GameManager``
  3. Modify the desired Training Phase and/or Training Type
  4. Enable Training CheckBox
  5. Set the desired amount of training environments using the slider (value of 1 equal to 9 environments)


### Adjusting Training Rewards/Penalties/Signals

To customize the reward/penalty values, perform the following steps:

![image](https://github.com/ChristosKrilisDev/ml-agents-thesis-project/assets/60070820/42fdaabc-0e39-4951-91bc-0ce907ab8c26)

1. Locate the ``RewardData.scriptableObject`` in the ``Data`` folder of the Project window (Asset/ML-Agents/PF/Data).
2. Open the ``RewardData`` scriptableObject you wish to change (there are 4 files, one for each phase)**.
3. Modify the following parameters:
	- ``Timer Value``: The time required to elapse before the agent receives a penalty for remaining on the same node.
	- ``Time Penalty Multiplier``: A factor applied to the penalty for staying on a node beyond the timer value.
	- ``Reward``: The reward value given to the agent.
	- ``Penalty``: The penalty value applied to the agent.
	- ``Use Revisited Penalty``: Toggle for enabling penalty when the agent revisits an old node.
	- ``Revisited Penalty``: The penalty triggered when the agent revisits a previous node.
	- ``Step Penalty``: The cumulative penalty the agent takes for the legnth of each episode.
	- ``Extra Distance``: Additional distance the agent have travel to find the optimal path before the episode terminates.
	- ``Step Reward Frequency``: Higher values result in reduced reward/distance.
	- ``Step Reward Multiple``: Multiplier applied to the step reward.
	- ``Epsilon``: Epsilon value for controlling exploration-exploitation trade-off.
	- ``Use Path Rewards Data``: Toggle for utilizing path rewards data.
	- ``Path Rewards Total Value``: The total reward value distributed among each path reward object (reward value/Spawn Path Rewards Counter).
	- ``Spawn Path Rewards Counter``: Counter for spawning path rewards.
	- ``Use Random Wall Width``: Toggle for randomizing wall widths.
	- ``Max Wall Width``: Maximum width for randomized walls.
	- ``BA`` (Block Areas): Blocks areas not part of the optimal path.
	- ``BAL`` (Block Area After Leaving): Blocks areas after the agent departs from a node.


## Agent and Environment Overview

The Agent utilizes Raycasting to gather information from its environment. You have the flexibility to adjust the Raycast values based on your requirements. Follow these steps to modify the Raycast values:

1. Open the ``Area`` prefab located in ``ML-Agents/Prefabs/Area``.
2. Within the prefab, select the ``Agent`` game object.
   - Alternatively, you can locate and select the Agent game object in the Hierarchy window. However, ensure you press ``Apply Change`` to update the changes to the prefab.


There are several key game object "tags" you should be aware of:

- ``Agent``: Represents the agent itself.
- ``Wall``: Tag assigned to deadly objects that the agent must avoid coming into contact with.
- ``Goal``: Tag assigned to objects that the agent needs to find.
- ``SwitchOn``: Tag assigned to the first goal object. When the agent touches it, the final goal is revealed.
- ``SwitchOff``: Tag assigned to the first goal object. It changes its value when the agent interacts with it.
- ``PFObj``: Tag assigned to small objects with rewards.
- ``SpawnArea``: Tag assigned to spawn areas or nodes.
- ``Reward``: Tag assigned to items with rewards.


## Visualize Dijkstra Tool

To visualize a specific area using the Dijkstra tool, follow these steps:


1. Open the ``Hierarchy`` window and locate the desired area you want to visualize.
2. Select the ``Graph-Dijkstra``, which is a child object of the ``AreaPF`` game object.
3. Press the ``Play`` button in Unity to activate the Dijkstra tool.

If, for any reason, the tool does not function properly, you can try preloading the Unity layout.

![image](https://github.com/ChristosKrilisDev/ml-agents-thesis-project/assets/60070820/68206dce-4413-49b8-9dd2-f6c7e2a49f29)


## Renaming The Models Tool

Once you have completed training your models, follow these steps to rename and organize them within the project:
1. Locate the ``Models`` Folder and open it.
    - ![image](https://github.com/ChristosKrilisDev/ml-agents-thesis-project/assets/60070820/868093f9-006d-447d-8a53-aac8085091a8)
3. By default, the trained models are saved in the ``<\ml-agents-thesis-project\results>`` directory, with the name "PF".
4. Drag and drop each trained model into the appropriate folder within the Models Folder.
5. To automatically rename the models, utilize the ``Rename NN-Models`` tool available in the top toolbar of the Unity Engine.
    - ![image](https://github.com/ChristosKrilisDev/ml-agents-thesis-project/assets/60070820/112d979c-69e8-4849-ba68-2f3dc14bb5ef)






# Start The Training

Before you begin training your model, ensure that you have followed the Getting Started Guide. 
Once you have done so, follow these steps:

1. Open a command line and navigate to the root folder of your project (the path should end with ..\ml-agents-thesis-project).
2. If you have installed the necessary Python packages and set up a Virtual Environment, activate it.
3. Set the desired Training Phase, Training Type, and the number of environments within Unity.
4. Ensure that the RL Training checkbox is checked.
5. Verify that the agent prefab has the Behavior Type set to Default. You can find this setting on the Behavior Parameters component attached to the agent game object.
6. In the command line, use the mlagents-learn commands (see the Training with Mlagents section above) to initiate the Python API.
7. Press the Play button in Unity to start the training process.
8. Training will commence, and once completed, you're done!


# Visualize Metrics


To observe the training process, whether during training or afterward, you can start TensorBoard and visualize the metrics by following these steps:

1. Open a command line or terminal window.
2. Navigate to the root folder of your project (the path should end with ..\ml-agents-thesis-project).
3. If you have installed the necessary Python packages and set up a Virtual Environment, activate it.
4. Execute the following command to start TensorBoard:
  - ```sh
    tensorboard --logdir results --port 6006
    ```
5. Open a web browser and navigate to [localhost:6006](http://localhost:6006) or click on the provided link in the command line output.
6. The TensorBoard web interface will load, allowing you to explore and visualize various training metrics, including graphs, histograms, scalars, and more.
7. Use the navigation and filtering options within TensorBoard to focus on specific metrics or compare different training sessions.
8. Keep TensorBoard running as long as you want to observe the training progress or review past training sessions.

Note: The ``results`` is a folder located to ``..\ml-agents-thesis-project\``.
