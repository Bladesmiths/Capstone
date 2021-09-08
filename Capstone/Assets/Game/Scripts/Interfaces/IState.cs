
namespace Bladesmiths.Capstone
{ 
public interface IState
{

    /// <summary>
    /// Is called on every state and acts like Update()
    /// </summary>
    void Tick();

    /// <summary>
    /// Called whenever the state is entered into
    /// </summary>
    void OnEnter();

    /// <summary>
    /// Called whenever the state is left
    /// </summary>
    void OnExit();


}
}
