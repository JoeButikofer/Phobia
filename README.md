# Phobia

The purpose of this project is to provide therapists with a tool to help treating phobias. The concept is to progressively expose the patient to his fear in a virtual reality to help him overcome it. 
While it is possible to use the same method in the real world, for example showing a spider to an arachnophobe, it can be really inconvenient or even impossible for some phobias. In addition, the therapist lack of control over the experience could prove to be dangerous. Using a simulation in VR allows us to handle almost all phobias while keeping a complete control over what is happening.

However, building a generic application that works for all phobias isn't possible simply because a phobia can be virtually anything. Some are relatively easy to simulate and can be abstracted to some extent, such as the fear of heights, darkness or a specific animal, while others (mostly situation-based phobias) demand more work and are much more specific, for example driving a car or getting an injection. There is often little in common between two phobias and for that reason, the solution developed is a platform providing base functionalities (avatar and VR management, handling world interactions, communication with the mobile app, ...) and allowing developers to add their implementation of a phobia (called a scenario).

To make the treatment progressive, therapists need to be able to tweak and adapt scenarios to their patients’ specific needs. To do that, scenarios can declare parameters in a manifest. The platform will then display these parameters to the user and handle the transfer of those values to the scenario when it is launched.
To serve as an example, the solution already contains 3 scenarios aimed at different stages of arachnophobia (the fear of spiders). This could be easily changed to another animal.

Regardless of the scenario, if something goes wrong the therapist can send the patient into a _safezone_, a personalized and relaxing environment. The _safezone_ is created by the therapist and his patient during their first session. The possibilities of the editor are limited to make it simple enough to be used by anyone. It is important to have an environment that will for sure calm the patient in case he panics.

Another application (Android) is installed on the therapist's mobile device. It allows him to control the simulation and take notes during the session. At the end of the session, those notes are saved alongside a video capture of the patient's point of view. Past sessions can then be reviewed inside the Unity application.


## Technologies

* Unity3D 5.5.1
* HTC Vive + controllers


