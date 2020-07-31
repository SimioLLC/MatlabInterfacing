function [result] = PlaySoundFile(filepath)
% PlayWaveFile - Given a filename, plan the file.
    %Play sound
    [y,Fs] = audioread(filepath);
    sound(y,Fs);
    
    result = "Success playing=" + filepath;
end