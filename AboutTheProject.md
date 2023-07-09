# Training with Mlagents

  
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


# Project Overview


This document provides instructions for modifying training settings and adjusting reward/penalty values of the project.


## Modifying Training Settings

To change the training phase or type, follow these steps:
  1.Open the ``Hierarchy`` window.
  2.Select the ``GameManager``.
  3.Modify the desired Training Phase and/or Training Type.
  4.Enable Training CheckBox.
  5.Set the desired amount of training environments using the slider (value of 1 equal to 9 environments)


## Adjusting Training Rewards/Penalties/Signals

To customize the reward/penalty values, perform the following steps:

![image](https://github.com/ChristosKrilisDev/ml-agents-thesis-project/assets/60070820/42fdaabc-0e39-4951-91bc-0ce907ab8c26)

  1.Locate the ``RewardData.scriptableObject`` in the ``Data`` folder of the Project window (Asset/ML-Agents/PF/Data).
  2.Open the ``RewardData`` scriptableObject you wish to change (there are 4 files, one for each phase)**.
  3.Modify the following parameters:
    -``Timer Value``: The time required to elapse before the agent receives a penalty for remaining on the same node.
    -``Time Penalty Multiplier``: A factor applied to the penalty for staying on a node beyond the timer value.
    -``Reward``: The reward value given to the agent.
    -``Penalty``: The penalty value applied to the agent.
    -``Use Revisited Penalty``: Toggle for enabling penalty when the agent revisits an old node.
		-``Revisited Penalty``: The penalty triggered when the agent revisits a previous node.
		-``Step Penalty``: The cumulative penalty incurred by the agent when the maximum step count is exceeded.!!
		-``Extra Distance``: Additional distance the agent must travel to find the optimal path.!!
		-``Step Reward Frequency``: Higher values result in reduced reward/distance.
		-``Step Reward Multiple``: Multiplier applied to the step reward.
		-``Epsilon``: Epsilon value for controlling exploration-exploitation trade-off.
		-``Use Path Rewards Data``: Toggle for utilizing path rewards data.
		-``Path Rewards Total Value``: The total reward value distributed among each path reward object (reward value/Spawn Path Rewards Counter).
		-``Spawn Path Rewards Counter``: Counter for spawning path rewards.
		-``Use Random Wall Width``: Toggle for randomizing wall widths.
		-``Max Wall Width``: Maximum width for randomized walls.
		-``BA`` (Block Areas): Blocks areas not part of the optimal path.
		-``BAL`` (Block Area After Leaving): Blocks areas after the agent departs from a node.
  


# Renaming The Models

After you finish training your models you can drag and drop them inside the Models Folder (see Screenshot)
![image](https://github.com/ChristosKrilisDev/ml-agents-thesis-project/assets/60070820/868093f9-006d-447d-8a53-aac8085091a8)


Normally, the trained models will be saved at ``<\ml-agents-thesis-project\results>``. The default name will be ``PF``. After you place each model on the correct folder, you can use the ``Rename NN-Models`` tool to automatically rename the models(The letter NN model will have an increased number in the end of the name)


You can find the tool on the Top tool bar that Unity Engine has.(See Screenshot)
![image](https://github.com/ChristosKrilisDev/ml-agents-thesis-project/assets/60070820/112d979c-69e8-4849-ba68-2f3dc14bb5ef)
