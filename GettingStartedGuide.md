# Installation Guide

You will need to: 
* [Install Unity](https://unity.com/download) (2019.4 or later)
* [Install Python](https://www.python.org/downloads/) (3.6.1 or higher)
    1. Installing PyTorch
    ```sh
    pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html
    ```
* Clone this repository (Optional)
* Install the com.unity.ml-agents Unity package (if missing)
    1. Navigating to the menu ``<Window>`` -> ``<Package Manager>``.
    2. Enable ``<Preview Packages>`` in the ``<Advanced>`` dropdown.
    4. Find the latest Preview release of the package.
    5. Hit Download and Install.
* Install the mlagents Python package
    1. Installing MLAgents
    ```sh
      python -m pip install mlagents==0.27.0
    ```
    2. if you installed this correctly, you should be able to run
    ```sh
       mlagents-learn --help
    ```
