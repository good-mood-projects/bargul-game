using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialSection {

    void setUpSection();
    bool waitForUserInteraction(); //Only if user confirmation is required
    bool waitForCondition();
    void loadMeteorMap(MeteorPattern[] newPatternList);
    void enableSection();

    void setLanguage(string value);
}
