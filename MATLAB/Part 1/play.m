%% This MALTAB script simply plays a sound clip called 'Ship.wav'
    % So whenever SIMIO calls this matlab script, this sould will be played.
    
    %Play sound
    filename= 'Ship.wav';
    [y,Fs] = audioread(filename);
    sound(y,Fs);