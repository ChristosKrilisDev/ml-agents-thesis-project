<img src="docs/images/image-banner.png" align="middle" width="3000"/>

# Training with mlagents-learn
To view a description of all the CLI options accepted by mlagents-learn, use the --help

```sh
mlagents-learn --help
```

The basic command for training is:
```sh
mlagents-learn <trainer-config-file> --env=<env_name> --run-id=<run-identifier>
```
1. Open a browser window and navigate to
   [localhost:6006](http://localhost:6006).

1. ``<trainer-config-file>`` is the file path of the trainer configuration YAML
2. ``<env_name>``(Optional) is the name (including path) of your Unity executable containing the agents to be trained.
3. ``<run-identifier>`` is a unique name you can use to identify the results of your training runs.
4. ``initialize-from=<run-identifier> --run-id=<new-id>`` start a new training run but initialize it using an already-trained model.
5. ``--force`` to re-run a previously interrupted or completed training run and re-use the same run ID.
6. ``--resume`` to resume a previously interrupted or completed training run

In order to observe the training process, either during training or afterward, start TensorBoard:
```sh
tensorboard --logdir results --port 6006
```


# Using RL To Solve Single-Source Shortest Path Problems In Game Environment


# This Project is using the Unity ML Agent Toolkit (see below)
**The Unity Machine Learning Agents Toolkit** (ML-Agents) is an open-source
project that enables games and simulations to serve as environments for
training intelligent agents. We provide implementations (based on PyTorch)
of state-of-the-art algorithms to enable game developers and hobbyists to easily
train intelligent agents for 2D, 3D and VR/AR games. Researchers can also use the
provided simple-to-use Python API to train Agents using reinforcement learning,
imitation learning, neuroevolution, or any other methods. These trained agents can be
used for multiple purposes, including controlling NPC behavior (in a variety of
settings such as multi-agent and adversarial), automated testing of game builds
and evaluating different game design decisions pre-release. The ML-Agents
Toolkit is mutually beneficial for both game developers and AI researchers as it
provides a central platform where advances in AI can be evaluated on Unity’s
rich environments and then made accessible to the wider research and game
developer communities.

([latest release](https://github.com/Unity-Technologies/ml-agents/releases/tag/latest_release))
([all releases](https://github.com/Unity-Technologies/ml-agents/releases))
[![docs badge](https://img.shields.io/badge/docs-reference-blue.svg)](https://github.com/Unity-Technologies/ml-agents/tree/release_19_docs/docs/)
[![license badge](https://img.shields.io/badge/license-Apache--2.0-green.svg)](LICENSE.md)

## Features

- Solves sssp in video games
- Use Dijkstra algorithm to validate the distance

## Documentation

  documentation that corresponds to the release version you're using.
- The `com.unity.ml-agents` package is [verified](https://docs.unity3d.com/2020.1/Documentation/Manual/pack-safe.html)
  for Unity 2020.1 and later. Verified packages releases are numbered 1.0.x.
  
  | **Training Type** | **Simple** | **Advanced** |
  |:-----------------:|:----------:|:------------:|
  | **Phase A**| ** as** || **aa ** |
  | **Phase B**|| **bs ** || **ab ** |
  | **Phase C**|| **cs ** || **ac ** |
  | **Phase D**|| **ds ** || **ad ** |


** SEE
If you use Unity or the ML-Agents Toolkit to conduct research, we ask that you
cite the following paper as a reference:

Juliani, A., Berges, V., Teng, E., Cohen, A., Harper, J., Elion, C., Goy, C.,
Gao, Y., Henry, H., Mattar, M., Lange, D. (2020). Unity: A General Platform for
Intelligent Agents. _arXiv preprint
[arXiv:1809.02627](https://arxiv.org/abs/1809.02627)._
https://github.com/Unity-Technologies/ml-agents.

## Additional Resources

We have a Unity Learn course,
[ML-Agents: Hummingbirds](https://learn.unity.com/course/ml-agents-hummingbirds),
that provides a gentle introduction to Unity and the ML-Agents Toolkit.

We've also partnered with
[CodeMonkeyUnity](https://www.youtube.com/c/CodeMonkeyUnity) to create a
[series of tutorial videos](https://www.youtube.com/playlist?list=PLzDRvYVwl53vehwiN_odYJkPBzcqFw110)
on how to implement and use the ML-Agents Toolkit.

We have also published a series of blog posts that are relevant for ML-Agents:

- (July 12, 2021)
  [ML-Agents plays Dodgeball](https://blog.unity.com/technology/ml-agents-plays-dodgeball)
- (May 5, 2021)
  [ML-Agents v2.0 release: Now supports training complex cooperative behaviors](https://blogs.unity3d.com/2021/05/05/ml-agents-v2-0-release-now-supports-training-complex-cooperative-behaviors/)
- (December 28, 2020)
  [Happy holidays from the Unity ML-Agents team!](https://blogs.unity3d.com/2020/12/28/happy-holidays-from-the-unity-ml-agents-team/)
- (November 20, 2020)
  [How Eidos-Montréal created Grid Sensors to improve observations for training agents](https://blogs.unity3d.com/2020/11/20/how-eidos-montreal-created-grid-sensors-to-improve-observations-for-training-agents/)
- (November 11, 2020)
  [2020 AI@Unity interns shoutout](https://blogs.unity3d.com/2020/11/11/2020-aiunity-interns-shoutout/)
- (May 12, 2020)
  [Announcing ML-Agents Unity Package v1.0!](https://blogs.unity3d.com/2020/05/12/announcing-ml-agents-unity-package-v1-0/)
- (February 28, 2020)
  [Training intelligent adversaries using self-play with ML-Agents](https://blogs.unity3d.com/2020/02/28/training-intelligent-adversaries-using-self-play-with-ml-agents/)
- (November 11, 2019)
  [Training your agents 7 times faster with ML-Agents](https://blogs.unity3d.com/2019/11/11/training-your-agents-7-times-faster-with-ml-agents/)
- (October 21, 2019)
  [The AI@Unity interns help shape the world](https://blogs.unity3d.com/2019/10/21/the-aiunity-interns-help-shape-the-world/)
- (April 15, 2019)
  [Unity ML-Agents Toolkit v0.8: Faster training on real games](https://blogs.unity3d.com/2019/04/15/unity-ml-agents-toolkit-v0-8-faster-training-on-real-games/)
- (March 1, 2019)
  [Unity ML-Agents Toolkit v0.7: A leap towards cross-platform inference](https://blogs.unity3d.com/2019/03/01/unity-ml-agents-toolkit-v0-7-a-leap-towards-cross-platform-inference/)
- (December 17, 2018)
  [ML-Agents Toolkit v0.6: Improved usability of Brains and Imitation Learning](https://blogs.unity3d.com/2018/12/17/ml-agents-toolkit-v0-6-improved-usability-of-brains-and-imitation-learning/)
- (October 2, 2018)
  [Puppo, The Corgi: Cuteness Overload with the Unity ML-Agents Toolkit](https://blogs.unity3d.com/2018/10/02/puppo-the-corgi-cuteness-overload-with-the-unity-ml-agents-toolkit/)
- (September 11, 2018)
  [ML-Agents Toolkit v0.5, new resources for AI researchers available now](https://blogs.unity3d.com/2018/09/11/ml-agents-toolkit-v0-5-new-resources-for-ai-researchers-available-now/)
- (June 26, 2018)
  [Solving sparse-reward tasks with Curiosity](https://blogs.unity3d.com/2018/06/26/solving-sparse-reward-tasks-with-curiosity/)
- (June 19, 2018)
  [Unity ML-Agents Toolkit v0.4 and Udacity Deep Reinforcement Learning Nanodegree](https://blogs.unity3d.com/2018/06/19/unity-ml-agents-toolkit-v0-4-and-udacity-deep-reinforcement-learning-nanodegree/)
- (May 24, 2018)
  [Imitation Learning in Unity: The Workflow](https://blogs.unity3d.com/2018/05/24/imitation-learning-in-unity-the-workflow/)
- (March 15, 2018)
  [ML-Agents Toolkit v0.3 Beta released: Imitation Learning, feedback-driven features, and more](https://blogs.unity3d.com/2018/03/15/ml-agents-v0-3-beta-released-imitation-learning-feedback-driven-features-and-more/)
- (December 11, 2017)
  [Using Machine Learning Agents in a real game: a beginner’s guide](https://blogs.unity3d.com/2017/12/11/using-machine-learning-agents-in-a-real-game-a-beginners-guide/)
- (December 8, 2017)
  [Introducing ML-Agents Toolkit v0.2: Curriculum Learning, new environments, and more](https://blogs.unity3d.com/2017/12/08/introducing-ml-agents-v0-2-curriculum-learning-new-environments-and-more/)
- (September 19, 2017)
  [Introducing: Unity Machine Learning Agents Toolkit](https://blogs.unity3d.com/2017/09/19/introducing-unity-machine-learning-agents/)
- Overviewing reinforcement learning concepts
  ([multi-armed bandit](https://blogs.unity3d.com/2017/06/26/unity-ai-themed-blog-entries/)
  and
  [Q-learning](https://blogs.unity3d.com/2017/08/22/unity-ai-reinforcement-learning-with-q-learning/))

### More from Unity

- [Unity Robotics](https://github.com/Unity-Technologies/Unity-Robotics-Hub)
- [Unity Computer Vision](https://unity.com/computer-vision)
- [Unity Game Simulation](https://unity.com/products/game-simulation)

## Privacy

In order to improve the developer experience for Unity ML-Agents Toolkit, we have added in-editor analytics.
Please refer to "Information that is passively collected by Unity" in the
[Unity Privacy Policy](https://unity3d.com/legal/privacy-policy).

## License

[Apache License 2.0](LICENSE.md)
