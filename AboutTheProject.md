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


# About the project


Select ``<GameManager>`` from ``<Hierachy>`` window.
  1. Change Training Phase or/and Training Type
  2. Enable Training


How to change Training rewards/penalty/signlas :

![image](https://github.com/ChristosKrilisDev/ml-agents-thesis-project/assets/60070820/42fdaabc-0e39-4951-91bc-0ce907ab8c26)


You can change the training reward/penalty values by selecting a Reward Data.scriptableObject.
Select ``<RewardData>`` from ``<Project>`` window in the Data folder (Asset/ML-Agents/PF/Data)
  1. Timer Value, the time needs to pass before agent gets penalty for staying on the same node
  2. Time penalty multiplier
  3. Reward
  4. Penalty
  5. Use Revisited Penalty
  6. Revisited Penalty, penalty triggered when agent revisit a old node
  7. Step Penalty, the total penalty agent will get over the Max steps count.
  8. Extra Distance, extra distance agent has to find the optimal path
  9. Step Reward Frequency, higher values => less reward/distance
  10. Step Reward Multiple
  11. Epsilon
  12. Use Path Rewards Data
  13. Path Rewards Total Value, the total reward (reward value/Spawn Path Rewards Counter) that each path reward object will have
  14. Spawn Path Rewards Counter
  15. Use Random Wall Width
  16. Max Wall Width, randomize walls
  17. BA, Block Area
  18. BAL, Block Area After leaving a node
