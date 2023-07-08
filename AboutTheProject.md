# Training with Mlagents
  
  documentation that corresponds to the release version you're using.
- The `com.unity.ml-agents` package is [verified](https://docs.unity3d.com/2020.1/Documentation/Manual/pack-safe.html)
  for Unity 2020.1 and later. Verified packages releases are numbered 1.0.x

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

You can change the training reward/penalty values by selecting a Reward Data.scriptableObject.
Select ``<RewardData>`` from ``<Project>`` window in the Data folder (Asset/ML-Agents/PF/Data)
  1. Timer Value (the time needs to pass before agent gets penalty for not changing node)
  2. Time penalty multiplier
  3. Reward
  4. Penalty
  5. Use Revisited Penalty (boolean)
  6. Revisited Penalty
  7. Step Penalty
  8. Extra Distance
  9. Step Reward Frequency
  10. Step Reward Multiple
  11. Epsilon
  12. Use Path Rewards Data
  13. Path Rewards Total Value
  14. Spawn Path Rewards Counter
  15. Use Random Wall Width
  16. Max Wall Width
  17. BA
  18. BAL
